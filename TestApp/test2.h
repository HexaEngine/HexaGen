
struct MyStruct
{
	float a, b;
};

#define EXTERN extern "C"
#define DLL_EXPORT __declspec(dllexport)
#define API_EXPORT DLL_EXPORT EXTERN

typedef int MyInt;
typedef MyInt MyInt2;
typedef MyInt2 MyInt3;
typedef MyInt3* MyIntPtr;  // This is a pointer!

void Test(MyIntPtr value);  // Should be int*, not MyIntPtr
void Test(MyIntPtr* value); // Should be int**, not MyIntPtr*

// Case 2: typedef'd pointer
typedef MyStruct* MyStructPtr;  // Cached: { BaseType: "MyStruct", PointerLevel: 1 }
void Test(MyStructPtr value);   // Resolve typedef → MyStruct*, final: MyStruct*
void Test(MyStructPtr* value);  // Outer loop: +1 pointer, resolve typedef → MyStruct* + 1 = MyStruct**, final: ref MyStruct*

// Case 3: const typedef'd pointer
typedef MyStruct* MyStructPtr;
void Test(const MyStructPtr value);  // const at level 0, resolve typedef adds level 1, final: ref MyStruct*
void Test(const MyStructPtr* value); // Outer +1 pointer, const at level 1, resolve adds level 1, final: in MyStruct*

enum MyEnum { };
typedef MyEnum MyEnum_t;
void Test(MyEnum_t value);
