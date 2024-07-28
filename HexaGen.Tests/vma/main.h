#define VMA_STATS_STRING_ENABLED 0
#define VMA_CALL_POST __cdecl

#if defined _WIN32 || defined __CYGWIN__
#define VMA_CALL_PRE __declspec(dllexport)
#else
#ifdef __GNUC__
#define VMA_CALL_PRE  __attribute__((__visibility__("default")))
#else
#define VMA_CALL_PRE
#endif
#endif

#ifdef _WIN32

#if !defined(NOMINMAX)
#define NOMINMAX
#endif

#if !defined(WIN32_LEAN_AND_MEAN)
#define WIN32_LEAN_AND_MEAN
#endif

#include <windows.h>
#if !defined(VK_USE_PLATFORM_WIN32_KHR)
#define VK_USE_PLATFORM_WIN32_KHR
#endif // #if !defined(VK_USE_PLATFORM_WIN32_KHR)

#else  // #ifdef _WIN32

#include <vulkan/vulkan.h>

#endif  // #ifdef _WIN32

#define VMA_IMPLEMENTATION

#include "vk_mem_alloc.h"
