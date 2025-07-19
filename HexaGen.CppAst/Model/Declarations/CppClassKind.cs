// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace HexaGen.CppAst.Model.Declarations
{
    /// <summary>
    /// Type of a <see cref="CppClass"/> (class, struct or union)
    /// </summary>
    public enum CppClassKind
    {
        /// <summary>
        /// A C++ `class`
        /// </summary>
        Class,
        /// <summary>
        /// A C++ `struct`
        /// </summary>
        Struct,
        /// <summary>
        /// A C++ `union`
        /// </summary>
        Union,
        /// <summary>
        /// An Objective-C `@interface`
        /// </summary>
        ObjCInterface,
        /// <summary>
        /// An Objective-C `@protocol`
        /// </summary>
        ObjCProtocol,
        /// <summary>
        /// An Objective-C `@interface` with a category.
        /// </summary>
        ObjCInterfaceCategory,
    }
}