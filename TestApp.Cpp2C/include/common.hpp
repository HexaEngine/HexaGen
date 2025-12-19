#pragma once
#include <atomic>
#include <cstdint>
#include <memory>
#include <type_traits>
#include <utility>
#include <stdexcept>
#include <algorithm>
#include <limits>
#include <vector>
#include <functional>
#include <string>
#include <stdexcept>
#include <iostream>
#include <array>

#define HEXA_PRISM_NAMESPACE HexaEngine::Prism
#define HEXA_PRISM_NAMESPACE_BEGIN namespace HexaEngine { namespace Prism {
#define HEXA_PRISM_NAMESPACE_END } }

#ifndef HEXA_MATH_NAMESPACE
#define HEXA_MATH_NAMESPACE Utils
#endif

#if defined(_WIN64) || defined(_WIN32)
#define HEXA_PRISM_WINDOWS 1
#endif

#define HEXA_PRISM_DEFINE_FLAG_OPERATORS(Type) \
constexpr Type operator|(const Type a, const Type b) { using U = std::underlying_type_t<Type>; return static_cast<Type>(static_cast<U>(a) | static_cast<U>(b)); } \
constexpr Type operator&(const Type a, const Type b) { using U = std::underlying_type_t<Type>; return static_cast<Type>(static_cast<U>(a) & static_cast<U>(b)); } \
constexpr Type operator^(const Type a, const Type b) { using U = std::underlying_type_t<Type>; return static_cast<Type>(static_cast<U>(a) ^ static_cast<U>(b)); } \
constexpr Type& operator|=(Type& a, const Type b) { a = a | b; return a; } \
constexpr Type& operator&=(Type& a, const Type b) { a = a & b; return a; } \
constexpr Type& operator^=(Type& a, const Type b) { a = a ^ b; return a; } \
constexpr Type operator~(const Type a) { using U = std::underlying_type_t<Type>; return static_cast<Type>(~static_cast<U>(a)); } \
constexpr bool operator==(const Type a, const Type b) { using U = std::underlying_type_t<Type>; return static_cast<U>(a) == static_cast<U>(b); } \
constexpr bool operator!=(const Type a, const Type b) { using U = std::underlying_type_t<Type>; return !(a == b); } \
constexpr bool operator==(const std::underlying_type_t<Type> a, const Type b) { using U = std::underlying_type_t<Type>; return a == static_cast<U>(b); } \
constexpr bool operator!=(const std::underlying_type_t<Type> a, const Type b) { return !(a == b); } \
constexpr bool operator==(const Type a, const std::underlying_type_t<Type> b) { using U = std::underlying_type_t<Type>; return static_cast<U>(a) == b; } \
constexpr bool operator!=(const Type a, const std::underlying_type_t<Type> b) { return !(a == b); }
