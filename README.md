# HexaGen

HexaGen is a comprehensive code generation toolkit for C# and C++ projects. It uses [HexaGen.CppAst](https://github.com/HexaEngine/HexaGen.CppAst) to parse C/C++ headers and automatically generates C# bindings and wrappers. HexaGen simplifies the process of integrating native libraries with C# applications by automating the creation of interop code.

## :sparkles: Features

- **C# Bindings for C Libraries:** Automatically generate C# bindings from C headers with support for functions, structs, enums, and callbacks
- **C# Bindings for COM Libraries:** Create C# bindings for COM interfaces and objects
- **C++ to C Wrappers:** Generate C wrappers around C++ libraries to facilitate C interop
- **Flexible Configuration:** JSON-based configuration system with inheritance and composition support
- **Advanced Function Generation:** Multiple parameter handling strategies including spans, refs, delegates, and default values
- **Type Mapping:** Configurable type mappings and conversions
- **Extension Methods:** Generate extension methods for improved API ergonomics
- **Constants to Enums:** Convert C preprocessor constants to strongly-typed C# enums

## :file_folder: Project Structure

- **HexaGen** - Main code generation tool and CLI
- **HexaGen.Core** - Core functionality and utilities for code generation
- **HexaGen.Cpp2C** - C++ to C wrapper generator
- **HexaGen.Runtime** - Runtime support library for generated code (multi-target: .NET 9/8/7/6, .NET Standard 2.0/2.1, .NET Framework 4.7.2, Android)
- **HexaGen.Runtime.COM** - Runtime support for COM interop (multi-target: .NET 9/8/7/6, .NET Standard 2.0/2.1, .NET Framework 4.7.2)
- **HexaGen.Language** - Language parsing and processing utilities
- **HexaGen.Tests** - Unit tests
- **HexaGen.PerformanceTests** - Performance benchmarks

## :wrench: Requirements

- **.NET SDK 9.0** (or compatible version)
- **Clang 17.0.4 or later** (for parsing C/C++ headers)
- **Visual Studio 2022 or later** (recommended for development)

## :package: Installation

### NuGet Package

Install HexaGen via NuGet Package Manager:

```bash
dotnet add package HexaGen
```

### Build from Source

1. **Clone the Repository:**
   ```bash
   git clone https://github.com/HexaEngine/HexaGen.git
   cd HexaGen
   ```

2. **Build the Project:**
   ```bash
   dotnet build
   ```

## :rocket: Usage

### Basic Example

Create a configuration file (e.g., `config.json`):

```json
{
  "ApiName": "MyLibrary",
  "Namespace": "MyLibrary.Generated",
  "ImportType": "DllImport",
  "GenerateExtensions": true
}
```

Generate C# bindings programmatically:

```csharp
using HexaGen;

var config = CsCodeGeneratorConfig.Load("config.json");
var generator = new CsCodeGenerator(config);
generator.Generate("mylibrary.h", "Output");
```

### :gear: Configuration

HexaGen uses a JSON-based configuration system that supports:

- **BaseConfig**: Configuration inheritance from other files
- **Type Mappings**: Custom type conversions
- **Function Rules**: Advanced parameter transformation rules
- **Constants to Enum**: Convert preprocessor defines to enums
- **Import Types**: DllImport, LibraryImport, or delegates

Example configuration with inheritance:

```json
{
  "BaseConfig": {
    "Url": "file://config.base.json",
    "IgnoredProperties": ["IgnoredTypes"]
  },
  "ApiName": "MyAPI",
  "Namespace": "MyAPI.Generated",
  "ImportType": "LibraryImport"
}
```

### C++ to C Generation

For generating C wrappers around C++ libraries:

```csharp
using HexaGen.Cpp2C;

var config = Cpp2CGeneratorConfig.Load("cpp2c-config.json");
var generator = new Cpp2CGenerator(config);
generator.Generate("mylibrary.hpp", "Output");
```

## :handshake: Contributing

Contributions are welcome! Please:

1. Fork the repository
2. Create a feature branch
3. Make your changes following the project's coding standards
4. Add appropriate tests
5. Submit a pull request

## :scroll: License

HexaGen is licensed under the MIT License. See the [LICENSE.txt](LICENSE.txt) file for details.

## :link: Links

- [GitHub Repository](https://github.com/HexaEngine/HexaGen)
- [NuGet Package](https://www.nuget.org/packages/HexaGen)
- [HexaGen.CppAst](https://github.com/HexaEngine/HexaGen.CppAst) - The underlying C++ parser (custom fork)
