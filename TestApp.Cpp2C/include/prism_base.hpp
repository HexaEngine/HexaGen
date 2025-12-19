#pragma once
#include "common.hpp"

HEXA_PRISM_NAMESPACE_BEGIN

class PrismObject
{
	std::atomic<size_t> counter;

public:
	PrismObject() : counter(1)
	{
	}

	void AddRef()
	{
		counter.fetch_add(1, std::memory_order_acq_rel);
	}

	void Release()
	{
		if (counter.fetch_sub(1, std::memory_order_acq_rel) == 1)
		{
			delete this; // TODO: Change to custom allocator solution.
		}
	}

	virtual ~PrismObject() = default;
};

template <typename T>
class PrismObj
{
	template <typename U> friend class PrismObj;
	T* ptr;

public:
	constexpr PrismObj() : ptr(nullptr)
	{
	}

	explicit PrismObj(T* p, bool addRef = true) noexcept : ptr(p)
	{
		if (ptr && addRef) ptr->AddRef();
	}

	PrismObj(const PrismObj& other) noexcept : ptr(other.ptr)
	{
		if (ptr) ptr->AddRef();
	}

	template <typename U, std::enable_if_t<std::is_convertible_v<U*, T*>, int> = 0>
	PrismObj(const PrismObj<U>& other) noexcept : ptr(other.Get())
	{
		if (ptr) ptr->AddRef();
	}

	template <typename U, std::enable_if_t<std::is_convertible_v<U*, T*>, int> = 0>
	PrismObj(PrismObj<U>& other) noexcept : ptr(other.Get())
	{
		if (ptr) ptr->AddRef();
	}

	PrismObj(PrismObj&& other) noexcept : ptr(other.ptr)
	{
		other.ptr = nullptr;
	}

	~PrismObj() noexcept
	{
		if (ptr) ptr->Release();
		ptr = nullptr;
	}

	PrismObj& operator=(const PrismObj& other)
	{
		if (this != &other)
		{
			if (other.ptr) other.ptr->AddRef();
			if (ptr) ptr->Release();
			ptr = other.ptr;
		}
		return *this;
	}

	template <typename U, std::enable_if_t<std::is_convertible_v<U*, T*>, int> = 0>
	PrismObj& operator=(PrismObj<U>& other)
	{
		if (ptr != other.ptr)
		{
			if (other.ptr) other.ptr->AddRef();
			if (ptr) ptr->Release();
			ptr = other.ptr;
		}
		return *this;
	}

	PrismObj& operator=(PrismObj&& other) noexcept
	{
		if (this != &other)
		{
			if (ptr) ptr->Release();
			ptr = other.ptr;
			other.ptr = nullptr;
		}
		return *this;
	}

	PrismObj& operator=(T* p) noexcept
	{
		if (ptr != p)
		{
			if (ptr) ptr->Release();
			p->AddRef();
			ptr = p;
		}
		return *this;
	}

	template <typename U>
	constexpr operator U* () { return ptr; }

	constexpr T* operator->() const { return ptr; }
	constexpr T& operator*() const { return *ptr; }
	constexpr operator bool() const noexcept { return ptr != nullptr; }
	bool operator==(const PrismObj<T>& other) const noexcept { return ptr == other.ptr; }
	bool operator!=(const PrismObj<T>& other) const noexcept { return ptr != other.ptr; }
	bool operator==(T* p) const noexcept { return ptr == p; }
	bool operator!=(T* p) const noexcept { return ptr != p; }

	constexpr T* Get() const { return ptr; }

	PrismObj<T> AddRef()
	{
		return PrismObj<T>(ptr, true);
	}

	T* Detach()
	{
		auto* tmp = ptr;
		ptr = nullptr;
		return tmp;
	}

	void Release()
	{
		if (ptr)
		{
			ptr->Release();
			ptr = nullptr;
		}
	}

	void Reset(T* ptr)
	{
		Release();
		this->ptr = ptr;
	}

	template <typename U>
	PrismObj<U> As() const
	{
		return PrismObj<U>(dynamic_cast<U*>(ptr), true);
	}

	template <typename U>
	U* AsPtr() const		
	{
		return dynamic_cast<U*>(ptr);
	}

	void swap(PrismObj<T>& other) noexcept
	{
		std::swap(ptr, other.ptr);
	}
};

template<typename TCallback>
class EventHandlerList
{
public:
	class EventHandler : public PrismObject
	{
		friend class EventHandlerList;
		EventHandlerList* list;
		TCallback callback;
		EventHandler* next;
		EventHandler* prev;

	public:
		EventHandler(EventHandlerList* list, TCallback callback, EventHandler* next, EventHandler* prev) : list(list), callback(std::move(callback)), next(next), prev(prev)
		{
		}

		EventHandlerList* GetList() const
		{
			return list;
		}

		void Unsubscribe()
		{
			if (list)
			{
				list->Unsubscribe(this);
				list = nullptr;
			}
		}
	};

	class EventHandlerToken
	{
		PrismObj<EventHandler> handler;

	public:
		EventHandlerToken() = default;
		EventHandlerToken(EventHandler* handler) : handler(handler)
		{
		}
		~EventHandlerToken()
		{
			Unsubscribe();
		}

		EventHandlerToken(const EventHandlerToken&) = delete;
		EventHandlerToken& operator=(const EventHandlerToken&) = delete;
		EventHandlerToken(EventHandlerToken&& other) noexcept : handler(std::move(other.handler))
		{
			other.handler = nullptr;
		}
		EventHandlerToken& operator=(EventHandlerToken&& other) noexcept
		{
			if (this != &other)
			{
				Unsubscribe();
				handler = std::move(other.handler);
				other.handler = nullptr;
			}
			return *this;
		}

		void Unsubscribe()
		{
			if (handler)
			{
				handler->Unsubscribe();
				handler = nullptr;
			}
		}
	};

private:
	EventHandler* head;
	std::atomic<size_t> lock;

	void Lock()
	{
		size_t value = lock.load(std::memory_order_relaxed);
		while (!lock.compare_exchange_weak(value, 1, std::memory_order_release, std::memory_order_acquire))
		{
			lock.wait(value, std::memory_order_relaxed);
		}
	}

	void Unlock()
	{
		lock.store(0, std::memory_order_release);
		lock.notify_one();
	}

	struct LockGuard
	{
		EventHandlerList* list;
		explicit LockGuard(EventHandlerList* list) : list(list)
		{
			list->Lock();
		}

		~LockGuard()
		{
			list->Unlock();
		}
	};

public:

	EventHandlerList() : head(nullptr)
	{
	}

	~EventHandlerList()
	{
		LockGuard guard(this);
		EventHandler* current = head;
		while (current)
		{
			EventHandler* next = current->next;
			current->list = nullptr; // Clear out get weak reference behavior, this will prevent any callback to call unsubscribe.
			current->Release();
			current = next;
		}
		head = nullptr;
	}

	EventHandlerToken Subscribe(TCallback callback)
	{
		LockGuard guard(this);
		EventHandler* newHandler = new EventHandler(this, std::move(callback), head, nullptr);
		if (head)
		{
			head->prev = newHandler;
		}
		head = newHandler;
		return EventHandlerToken(newHandler);
	}

	void Unsubscribe(EventHandler* handler)
	{
		LockGuard guard(this);
		if (handler->prev)
		{
			handler->prev->next = handler->next;
		}
		else
		{
			head = handler->next;
		}
		if (handler->next)
		{
			handler->next->prev = handler->prev;
		}
		handler->Release();
	}

	template<typename... TArgs>
	void Invoke(TArgs&&... args)
	{
		LockGuard guard(this);
		EventHandler* current = head;
		while (current)
		{
			current->callback(std::forward<TArgs>(args)...);
			current = current->next;
		}
	}
};

[[nodiscard]] inline void* PrismAlloc(const size_t size)
{
	return malloc(size);
}

inline void PrismFree(void* ptr)
{
	free(ptr);
}

template <typename T>
[[nodiscard]] inline T* PrismAllocT(const size_t count)
{
	return static_cast<T*>(PrismAlloc(sizeof(T) * count));
}

inline void PrismZeroMemory(void* mem, const size_t size)
{
	std::memset(mem, 0, size);
}

template <typename T>
inline void PrismZeroMemoryT(T* mem, const size_t count)
{
	std::memset(mem, 0, sizeof(T) * count);
}

inline void PrismMemoryCopy(void* dst, const void* src, size_t size)
{
	std::memcpy(dst, src, size);
}

template <typename T>
inline void PrismMemoryCopyT(T* dst, const T* src, size_t count)
{
	std::memcpy(dst, src, sizeof(T) * count);
}

template <typename T, typename... TArgs>
[[nodiscard]] inline PrismObj<T> MakePrismObj(TArgs&&... args)
{
	T* mem = PrismAllocT<T>(1);
	T* obj = new(mem) T(std::forward<TArgs>(args)...);
	return PrismObj<T>(obj, false);
}


template<typename T>
class uptr
{
	T* ptr;

public:
	constexpr uptr() noexcept : ptr(nullptr) {}
	
	explicit uptr(T* p) noexcept : ptr(p) {}
	
	~uptr()
	{
		if (ptr)
		{
			ptr->~T();
			PrismFree(ptr);
		}
	}

	uptr(const uptr&) = delete;
	uptr& operator=(const uptr&) = delete;

	uptr(uptr&& other) noexcept : ptr(other.ptr)
	{
		other.ptr = nullptr;
	}

	uptr& operator=(uptr&& other) noexcept
	{
		if (this != &other)
		{
			if (ptr)
			{
				ptr->~T();
				PrismFree(ptr);
			}
			ptr = other.ptr;
			other.ptr = nullptr;
		}
		return *this;
	}

	constexpr T* operator->() const noexcept { return ptr; }
	constexpr T& operator*() const noexcept { return *ptr; }
	constexpr explicit operator bool() const noexcept { return ptr != nullptr; }
	
	constexpr T* get() const noexcept { return ptr; }

	T* release() noexcept
	{
		T* tmp = ptr;
		ptr = nullptr;
		return tmp;
	}

	void reset(T* p = nullptr) noexcept
	{
		if (ptr)
		{
			ptr->~T();
			PrismFree(ptr);
		}
		ptr = p;
	}

	void swap(uptr& other) noexcept
	{
		std::swap(ptr, other.ptr);
	}
};

namespace detail
{
	template<typename T>
	inline void destroy_range(T* ptr, size_t count)
	{
		if constexpr (!std::is_trivially_destructible_v<T>)
		{
			for (size_t i = 0; i < count; ++i)
			{
				ptr[i].~T();
			}
		}
	}

	template<typename T>
	inline void default_construct_range(T* ptr, size_t count)
	{
		if constexpr (std::is_trivially_default_constructible_v<T>)
		{
			PrismZeroMemoryT(ptr, count);
		}
		else
		{
			for (size_t i = 0; i < count; ++i)
			{
				new (ptr + i) T();
			}
		}
	}

	template<typename T>
	inline void copy_construct_range(T* dst, const T* src, size_t count)
	{
		if constexpr (std::is_trivially_copy_constructible_v<T>)
		{
			PrismMemoryCopyT(dst, src, count);
		}
		else
		{
			for (size_t i = 0; i < count; ++i)
			{
				new (dst + i) T(src[i]);
			}
		}
	}

	template<typename T>
	inline void move_construct_range(T* dst, T* src, size_t count)
	{
		if constexpr (std::is_trivially_move_constructible_v<T>)
		{
			PrismMemoryCopyT(dst, src, count);
		}
		else
		{
			for (size_t i = 0; i < count; ++i)
			{
				new (dst + i) T(std::move(src[i]));
			}
		}
	}

	template<typename T>
	inline void value_construct_range(T* ptr, size_t count, const T& value)
	{
		for (size_t i = 0; i < count; ++i)
		{
			new (ptr + i) T(value);
		}
	}
}

template <typename T>
class uarray
{
	T* ptr;
	size_t count;

public:
	constexpr uarray() noexcept : ptr(nullptr), count(0) {}
	
	explicit uarray(T* p, size_t n) noexcept : ptr(p), count(n) {}
	
	~uarray()
	{
		if (ptr)
		{
			detail::destroy_range(ptr, count);
			PrismFree(ptr);
		}
	}

	uarray(const uarray&) = delete;
	uarray& operator=(const uarray&) = delete;

	uarray(uarray&& other) noexcept : ptr(other.ptr), count(other.count)
	{
		other.ptr = nullptr;
		other.count = 0;
	}

	uarray& operator=(uarray&& other) noexcept
	{
		if (this != &other)
		{
			if (ptr)
			{
				detail::destroy_range(ptr, count);
				PrismFree(ptr);
			}
			ptr = other.ptr;
			count = other.count;
			other.ptr = nullptr;
			other.count = 0;
		}
		return *this;
	}

	constexpr T& operator[](size_t index) noexcept { return ptr[index]; }
	constexpr const T& operator[](size_t index) const noexcept { return ptr[index]; }
	constexpr explicit operator bool() const noexcept { return ptr != nullptr; }
	
	constexpr T* get() const noexcept { return ptr; }
	constexpr size_t size() const noexcept { return count; }
	constexpr T* data() const noexcept { return ptr; }

	T* begin() noexcept { return ptr; }
	const T* begin() const noexcept { return ptr; }
	T* end() noexcept { return ptr + count; }
	const T* end() const noexcept { return ptr + count; }

	T* release() noexcept
	{
		T* tmp = ptr;
		ptr = nullptr;
		count = 0;
		return tmp;
	}

	void reset(T* p = nullptr, size_t n = 0) noexcept
	{
		if (ptr)
		{
			detail::destroy_range(ptr, count);
			PrismFree(ptr);
		}
		ptr = p;
		count = n;
	}

	void swap(uarray& other) noexcept
	{
		std::swap(ptr, other.ptr);
		std::swap(count, other.count);
	}
};

template <typename T>
class container
{
protected:
	T* ptr;
	size_t size_m;

public:
	constexpr container() noexcept : ptr(nullptr), size_m(0) {}
	
	explicit container(size_t n) : ptr(nullptr), size_m(n)
	{
		if (n > 0)
		{
			ptr = PrismAllocT<T>(n);
			detail::default_construct_range(ptr, n);
		}
	}

	container(size_t n, const T& value) : ptr(nullptr), size_m(n)
	{
		if (n > 0)
		{
			ptr = PrismAllocT<T>(n);
			detail::value_construct_range(ptr, n, value);
		}
	}
	
	~container()
	{
		if (ptr)
		{
			detail::destroy_range(ptr, size_m);
			PrismFree(ptr);
		}
	}

	container(const container& other) : ptr(nullptr), size_m(other.size_m)
	{
		if (other.size_m > 0)
		{
			ptr = PrismAllocT<T>(size_m);
			detail::copy_construct_range(ptr, other.ptr, size_m);
		}
	}

	container& operator=(const container& other)
	{
		if (this != &other)
		{
			if (ptr)
			{
				detail::destroy_range(ptr, size_m);
				PrismFree(ptr);
				ptr = nullptr;
			}
			
			size_m = other.size_m;
			if (size_m > 0)
			{
				ptr = PrismAllocT<T>(size_m);
				detail::copy_construct_range(ptr, other.ptr, size_m);
			}
		}
		return *this;
	}

	container(container&& other) noexcept : ptr(other.ptr), size_m(other.size_m)
	{
		other.ptr = nullptr;
		other.size_m = 0;
	}

	container& operator=(container&& other) noexcept
	{
		if (this != &other)
		{
			if (ptr)
			{
				detail::destroy_range(ptr, size_m);
				PrismFree(ptr);
			}
			ptr = other.ptr;
			size_m = other.size_m;
			other.ptr = nullptr;
			other.size_m = 0;
		}
		return *this;
	}

	constexpr T& operator[](size_t index) noexcept { return ptr[index]; }
	constexpr const T& operator[](size_t index) const noexcept { return ptr[index]; }
	constexpr explicit operator bool() const noexcept { return ptr != nullptr; }
	
	constexpr T* get() const noexcept { return ptr; }
	constexpr size_t size() const noexcept { return size_m; }
	constexpr T* data() const noexcept { return ptr; }
	constexpr bool empty() const noexcept { return size_m == 0; }

	T* begin() noexcept { return ptr; }
	const T* begin() const noexcept { return ptr; }
	T* end() noexcept { return ptr + size_m; }
	const T* end() const noexcept { return ptr + size_m; }

	T& front() { return ptr[0]; }
	const T& front() const { return ptr[0]; }
	T& back() { return ptr[size_m - 1]; }
	const T& back() const { return ptr[size_m - 1]; }

	void resize(size_t newSize)
	{
		if (newSize == size_m)
			return;

		T* newPtr = nullptr;
		if (newSize > 0)
		{
			newPtr = PrismAllocT<T>(newSize);
			
			size_t copyCount = newSize < size_m ? newSize : size_m;
			detail::move_construct_range(newPtr, ptr, copyCount);
			detail::default_construct_range(newPtr + copyCount, newSize - copyCount);
		}
		
		if (ptr)
		{
			detail::destroy_range(ptr, size_m);
			PrismFree(ptr);
		}
		
		ptr = newPtr;
		size_m = newSize;
	}

	void resize(size_t newSize, const T& value)
	{
		if (newSize == size_m)
			return;

		T* newPtr = nullptr;
		if (newSize > 0)
		{
			newPtr = PrismAllocT<T>(newSize);
			
			size_t copyCount = newSize < size_m ? newSize : size_m;
			detail::move_construct_range(newPtr, ptr, copyCount);
			
			if (newSize > size_m)
			{
				detail::value_construct_range(newPtr + copyCount, newSize - copyCount, value);
			}
		}
		
		if (ptr)
		{
			detail::destroy_range(ptr, size_m);
			PrismFree(ptr);
		}
		
		ptr = newPtr;
		size_m = newSize;
	}

	void push_back(const T& value)
	{
		T* newPtr = PrismAllocT<T>(size_m + 1);
		
		detail::move_construct_range(newPtr, ptr, size_m);
		new (newPtr + size_m) T(value);
		
		if (ptr)
		{
			detail::destroy_range(ptr, size_m);
			PrismFree(ptr);
		}
		
		ptr = newPtr;
		++size_m;
	}

	void push_back(T&& value)
	{
		T* newPtr = PrismAllocT<T>(size_m + 1);
		
		detail::move_construct_range(newPtr, ptr, size_m);
		new (newPtr + size_m) T(std::move(value));
		
		if (ptr)
		{
			detail::destroy_range(ptr, size_m);
			PrismFree(ptr);
		}
		
		ptr = newPtr;
		++size_m;
	}

	template<typename... Args>
	void emplace_back(Args&&... args)
	{
		T* newPtr = PrismAllocT<T>(size_m + 1);
		
		detail::move_construct_range(newPtr, ptr, size_m);
		new (newPtr + size_m) T(std::forward<Args>(args)...);
		
		if (ptr)
		{
			detail::destroy_range(ptr, size_m);
			PrismFree(ptr);
		}
		
		ptr = newPtr;
		++size_m;
	}

	void pop_back()
	{
		if (size_m > 0)
		{
			resize(size_m - 1);
		}
	}

	void clear()
	{
		if (ptr)
		{
			detail::destroy_range(ptr, size_m);
			PrismFree(ptr);
			ptr = nullptr;
		}
		size_m = 0;
	}

	void swap(container& other) noexcept
	{
		std::swap(ptr, other.ptr);
		std::swap(size_m, other.size_m);
	}
};

template <typename T, typename... TArgs>
uptr<T> make_uptr(TArgs&&... args)
{
	T* mem = PrismAllocT<T>(1);
	return uptr<T>(new (mem) T(std::forward<TArgs>(args)...));
}

template <typename T>
uarray<T> make_uarray(size_t count)
{
	T* mem = PrismAllocT<T>(count);
	detail::default_construct_range(mem, count);
	return uarray<T>(mem, count);
}

template <typename T>
uarray<T> make_uarray_uninitialized(size_t count)
{
	T* mem = PrismAllocT<T>(count);
	return uarray<T>(mem, count);
}

class String : public container<char>
{
public:
	constexpr String() = default;
	
	String(const char* str)
	{
		if (str)
		{
			size_m = std::strlen(str);
			ptr = PrismAllocT<char>(size_m + 1);
			PrismMemoryCopy(ptr, str, size_m);
			ptr[size_m] = '\0';
		}
	}

	String(const String& other)
	{
		if (other.ptr && other.size_m > 0)
		{
			size_m = other.size_m;
			ptr = PrismAllocT<char>(size_m + 1);
			PrismMemoryCopy(ptr, other.ptr, size_m);
			ptr[size_m] = '\0';
		}
	}

	String& operator=(const String& other)
	{
		if (this != &other)
		{
			if (ptr)
			{
				PrismFree(ptr);
				ptr = nullptr;
			}
			
			if (other.ptr && other.size_m > 0)
			{
				size_m = other.size_m;
				ptr = PrismAllocT<char>(size_m + 1);
				PrismMemoryCopy(ptr, other.ptr, size_m);
				ptr[size_m] = '\0';
			}
			else
			{
				size_m = 0;
			}
		}
		return *this;
	}

	String(String&& other) noexcept : container<char>(std::move(other))
	{
	}

	String& operator=(String&& other) noexcept
	{
		container<char>::operator=(std::move(other));
		return *this;
	}

	const char* c_str() const
	{
		return ptr ? ptr : "";
	}
};

class DeviceChild : public PrismObject
{
public:
	virtual void* GetNativePointer() = 0;
};

HEXA_PRISM_NAMESPACE_END