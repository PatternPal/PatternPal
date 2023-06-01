﻿#region

using PatternPal.SyntaxTree.Models;

using static PatternPal.Core.Checks.CheckBuilder;

#endregion

namespace PatternPal.Core.Recognizers;

/// <summary>
/// A method which creates a lot of <see cref="ICheck"/>s that each adheres to the requirements a factory-method pattern needs to have implemented.
/// It returns the requirements in a tree structure stated per class.
/// </summary>
/// <remarks>
/// Requirements for the Product class:<br/>
///     a) is an interface<br/>
///     b) gets inherited by at least one class<br/>
///     c) gets inherited by at least two classes<br/>
/// Requirements for the Concrete Product class:<br/>
///     a) inherits Product<br/>
///     b) gets created in a Concrete Creator<br/>
/// Requirements for the Creator class:<br/>
///     a) is an abstract class<br/>
///     b) gets inherited by at least one class<br/>
///     c) gets inherited by at least two classes<br/>
///     d) contains a factory-method with the following properties<br/>
///         1) method is abstract<br/>
///         2) method is public<br/>
///         3) returns an object of type Product<br/>
/// Requirements for the Concrete Creator class:<br/>
///     a) inherits Creator<br/>
///     b) has exactly one method that returns an Concrete Product<br/>
/// </remarks>
internal class FactoryMethodRecognizer : IRecognizer
{
    /// <inheritdoc />
    public string Name => "Factory-Method";

    /// <inheritdoc />
    public Recognizer RecognizerType => Recognizer.FactoryMethod;

    /// <summary>
    /// A method which creates a lot of <see cref="ICheck"/>s that each adheres to the requirements a singleton pattern needs to have implemented.
    /// It returns the requirements in a tree structure stated per class.
    /// </summary>
    public IEnumerable< ICheck > Create()
    {
        
    }

    
}
