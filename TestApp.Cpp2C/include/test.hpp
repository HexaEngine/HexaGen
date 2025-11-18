
class Test
{
	float value;
public:
	Test(float value);
	~Test();

	void Method();

	float GetValue() const;

	void SetValue(float value);

};

inline float Add(float a, float b)
{
	return a + b;
}

inline float Subtract(float a, float b)
{
	return a - b;
}