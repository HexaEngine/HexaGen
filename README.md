# HexaGen

HexaGen is a code generation tool designed to generate bindings for C# for C and COM libraries using [CppAst.NET](https://github.com/xoofx/CppAst.NET). Additionally, it includes a generator for creating wrappers around C++ libraries for C. HexaGen aims to simplify the process of integrating native libraries with C# applications.

## Features

- **C# Bindings for C Libraries:** Automatically generate C# bindings for C libraries.
- **C# Bindings for COM Libraries:** Easily create C# bindings for COM libraries.
- **C++ to C Wrappers:** Generate C wrappers for C++ libraries, facilitating their use in C projects.

## Work in Progress

- **Cpp to C Generator:** A feature under development to create wrappers around C++ libraries for C.

## Requirements

To build and use HexaGen, ensure you have the following tools installed:

- **.NET SDK 8.0**
- **Clang 17.0.4 or later**
- **Visual Studio 2022 or later**

## Build Instructions

To build HexaGen, follow these steps:

1. **Install .NET SDK 8.0:**
   Download and install the .NET SDK 8.0 from the official [.NET website](https://dotnet.microsoft.com/download/dotnet/8.0).

2. **Install Clang:**
   Ensure you have Clang 17.0.4 or later installed. You can download it from the [official Clang website](https://clang.llvm.org/).

3. **Install Visual Studio 2022:**
   Download and install Visual Studio 2022 or later from the [Visual Studio website](https://visualstudio.microsoft.com/).

4. **Clone the Repository:**
   Clone the HexaGen repository from GitHub to your local machine:
   ```bash
   git clone https://github.com/HexaEngine/HexaGen.git
   cd HexaGen
   ```
5. **Build the Project:**
   Open the project in Visual Studio 2022 and build the solution, or use the .NET CLI:
   ```bash
   dotnet build
   ```

## Usage

Instructions on how to use HexaGen will be provided once the project reaches a stable release. Stay tuned for updates!

## Contributing

Contributions are welcome! Please fork the repository and submit a pull request with your changes. Ensure that your code follows the project's coding standards and includes appropriate tests.

## License

HexaGen is licensed under the MIT License. See the LICENSE file for more details.
