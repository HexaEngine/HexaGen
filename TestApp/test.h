
struct MyStruct
{
	float a, b;
};

#define EXTERN extern "C"
#define DLL_EXPORT __declspec(dllexport)
#define API_EXPORT DLL_EXPORT EXTERN

// Basic const variations
API_EXPORT void Test1(const int* value);              // in int
API_EXPORT void Test2(int* const value);              // ref int (const pointer, not const data)
API_EXPORT void Test3(const int* const value);        // in int (both const)

// Multi-level pointers
API_EXPORT void Test4(int** value);                   // ref int*
API_EXPORT void Test5(const int** value);             // in int* (const at wrong level)
API_EXPORT void Test6(int* const* value);             // ref int*
API_EXPORT void Test7(const int* const* value);       // ref int* (const at first pointer)
API_EXPORT void Test8(int*** value);                  // ref int**

// void pointers
API_EXPORT void Test9(void* value);                   // void*
API_EXPORT void Test10(const void* value);            // void*
API_EXPORT void Test11(void** value);                 // ref void* or ref nint
API_EXPORT void Test12(const void** value);           // ref void* or in void*

// Structs with multiple pointer levels
API_EXPORT void Test13(MyStruct** value);             // ref MyStruct*
API_EXPORT void Test14(const MyStruct** value);       // in MyStruct* or ref MyStruct*?
API_EXPORT void Test15(MyStruct* const* value);       // ref MyStruct*
API_EXPORT void Test16(const MyStruct* const* value); // ref MyStruct*

// References (should behave like pointers)
API_EXPORT void Test17(MyStruct& value);              // ref MyStruct
API_EXPORT void Test18(const MyStruct& value);        // in MyStruct
API_EXPORT void Test19(MyStruct*& value);             // ref MyStruct*
API_EXPORT void Test20(const MyStruct*& value);       // ref MyStruct* or in MyStruct*?

// Arrays
API_EXPORT void Test21(int arr[10]);                  // int* or fixed array?
API_EXPORT void Test22(const int arr[10]);            // int* or in?
API_EXPORT void Test23(int arr[][10]);                // int* (multidimensional decay)

// Function pointers
typedef void (*Callback)(int);
API_EXPORT void Test24(Callback cb);                  // Raw: delegate*<int, void>, Ref: CallbackDelegate
API_EXPORT void Test25(Callback* cb);                 // Raw: delegate*<int, void>*, Ref: ref CallbackDelegate
API_EXPORT void Test26(const Callback* cb);           // Raw: delegate*<int, void>*, Ref: in CallbackDelegate

// Enums
enum MyEnum { A, B, C };
API_EXPORT void Test27(MyEnum value);                 // MyEnum
API_EXPORT void Test28(MyEnum* value);                // ref MyEnum
API_EXPORT void Test29(const MyEnum* value);          // in MyEnum

// Typedef chains
typedef int MyInt;
typedef MyInt* MyIntPtr;
API_EXPORT void Test30(MyIntPtr value);               // int*
API_EXPORT void Test31(const MyIntPtr value);         // int* (const pointer)
API_EXPORT void Test32(MyIntPtr* value);              // ref int*

// Edge case: const at base type (not pointer level)
API_EXPORT void Test33(const int value);              // int (const ignored for value types)
API_EXPORT void Test34(const MyStruct value);         // MyStruct (const ignored)

// Volatile (if you handle it)
API_EXPORT void Test35(volatile int* value);          // ref int (volatile?)
API_EXPORT void Test36(const volatile int* value);    // in int (const + volatile?)

// Opaque/forward declared types
struct OpaqueType;
API_EXPORT void Test37(OpaqueType* value);            // OpaqueType*
API_EXPORT void Test38(const OpaqueType* value);      // in OpaqueType

// Mixed const scenarios
API_EXPORT void Test39(const int* const* const value); // Multiple consts
API_EXPORT void Test40(int* const* const* value);      // Const at wrong levels

// Return types (bonus)
API_EXPORT int* Test41();                             // int*
API_EXPORT const int* Test42();                       // int* (const return pointer)
API_EXPORT const MyStruct* Test43();                  // MyStruct*

// Tricky: pointer to const pointer
API_EXPORT void Test44(const int** const value);      // What should this be?
API_EXPORT void Test45(const MyStruct* const* value); // ref MyStruct* with const at level 1?