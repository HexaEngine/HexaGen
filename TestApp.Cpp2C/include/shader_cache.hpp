#pragma once
#include "prism.hpp"

HEXA_PRISM_NAMESPACE_BEGIN

class ReaderWriterLock
{
	std::atomic<size_t> val;
	static constexpr size_t WriterBit = size_t(1) << (sizeof(size_t) * 8 - 1);
	static constexpr size_t ReaderMask = ~(size_t)WriterBit;

	void WaitForReaders()
	{
		while (true)
		{
			auto expected = val.load(std::memory_order_acquire);
			if ((expected & ReaderMask) == 0)
			{
				break;
			}
			val.wait(expected, std::memory_order_relaxed);
		}
	}
	
public:
	bool try_lock()
	{
		auto prev = val.fetch_or(WriterBit, std::memory_order_acq_rel);
		if ((prev & WriterBit) != 0)
		{
			return false;
		}
		
		WaitForReaders();
		return true;
	}

	void lock()
	{
		while (true)
		{
			auto prev = val.fetch_or(WriterBit, std::memory_order_acq_rel);
			if ((prev & WriterBit) == 0)
			{
				break;
			}
			val.wait(prev, std::memory_order_relaxed);
		} 

		WaitForReaders();
	}

	void unlock()
	{
		val.fetch_and(~WriterBit, std::memory_order_release);
		val.notify_all();
	}

	bool try_lock_shared()
	{
		auto expected = val.load(std::memory_order_acquire);
		expected &= ~WriterBit;
		return val.compare_exchange_strong(expected, expected + 1, std::memory_order_release, std::memory_order_acquire);
	}

	void lock_shared()
	{
		while (true)
		{
			auto expected = val.load(std::memory_order_acquire);
			expected &= ~WriterBit;
			if (val.compare_exchange_weak(expected, expected + 1, std::memory_order_release, std::memory_order_acquire))
			{
				break;
			}
			val.wait(expected, std::memory_order_relaxed);
		}
	}

	void unlock_shared()
	{
		val.fetch_sub(1, std::memory_order_release);
		val.notify_all();
	}
};

class ShaderCache
{
	struct ShaderCacheEntry
	{
		std::atomic<size_t> lock;
		char* key;
		PrismObj<Blob> shader;


	};

	std::atomic<size_t> lock;

public:
	ShaderCache() = default;
	~ShaderCache() = default;
	PrismObj<Blob> GetShader(const char* key) const;
	void SetShader(const char* key, Blob* shader);

private:
	void BeginRead();
	void EndRead();
	void BeginWrite();
	void EndWrite();
};

HEXA_PRISM_NAMESPACE_END