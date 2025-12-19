typedef void (*TestCallback)(int* data);
void TestCallbackFunc(TestCallback callback, void* data);
void GetCallbackFunc(TestCallback* outCallback);

void AnonymousCallbackFunc(float(*values_getter)(void* data, int idx));