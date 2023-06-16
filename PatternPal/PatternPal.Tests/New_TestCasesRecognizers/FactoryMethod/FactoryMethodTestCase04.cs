namespace PatternPal.Tests.TestClasses.FactoryMethod
{
    /* Pattern:              Factory method
     * Original code source: https://github.com/exceptionnotfound/DesignPatterns/blob/master/FactoryMethodPattern
     *
     * Requirements to fullfill the pattern:
     *         Product
     *               a) is an interface
     *            ✓  b) gets inherited by at least one class
     *            ✓  c) gets inherited by at least two classes
     *         Concrete Product
     *            ✓  a) inherits Product
     *            ✓  b) gets created in a Concrete Creator
     *         Creator
     *            ✓  a) is an abstract class
     *            ✓  b) gets inherited by at least one class 
     *            ✓  c) gets inherited by at least two classes
     *               d) contains a factory-method with the following properties
     *            ✓        1) method is abstract
     *            ✓        2) method is public
     *                     3) returns an object of type Product
     *         Concrete Creator
     *            ✓  a) inherits Creator
     *               b) has exactly one method that creates and returns a Concrete product
     */

    // Product
    internal abstract class Ingredient
    {
    }

    // Concrete product
    internal class Lettuce : Ingredient
    {
    }

    // Concrete product
    internal class Bread : Ingredient
    {
    }

    // Concrete product
    internal class Mayonnaise : Ingredient
    {
    }

    // Concrete product
    internal class Turkey : Ingredient
    {
    }

    // Creator
    internal abstract class Sandwich
    {
        public Sandwich()
        {
            CreateIngredients();
        }

        public List<Ingredient> Ingredients { get; } = new List<Ingredient>();

        // Factory method
        public abstract void CreateIngredients();
    }

    // Concrete creator
    internal class TurkeySandwich : Sandwich
    {
        public override void CreateIngredients()
        {
            Ingredients.Add(new Bread());
            Ingredients.Add(new Mayonnaise());
            Ingredients.Add(new Lettuce());
            Ingredients.Add(new Turkey());
            Ingredients.Add(new Turkey());
            Ingredients.Add(new Bread());
        }
    }

    // Concrete creator
    internal class Dagwood : Sandwich //OM NOM NOM
    {
        public override void CreateIngredients()
        {
            Ingredients.Add(new Bread());
            Ingredients.Add(new Turkey());
            Ingredients.Add(new Turkey());
            Ingredients.Add(new Lettuce());
            Ingredients.Add(new Lettuce());
            Ingredients.Add(new Mayonnaise());
            Ingredients.Add(new Bread());
            Ingredients.Add(new Turkey());
            Ingredients.Add(new Turkey());
            Ingredients.Add(new Lettuce());
            Ingredients.Add(new Lettuce());
            Ingredients.Add(new Mayonnaise());
            Ingredients.Add(new Bread());
            Ingredients.Add(new Turkey());
            Ingredients.Add(new Turkey());
            Ingredients.Add(new Lettuce());
            Ingredients.Add(new Lettuce());
            Ingredients.Add(new Mayonnaise());
            Ingredients.Add(new Bread());
            Ingredients.Add(new Turkey());
            Ingredients.Add(new Turkey());
            Ingredients.Add(new Lettuce());
            Ingredients.Add(new Lettuce());
            Ingredients.Add(new Mayonnaise());
            Ingredients.Add(new Bread());
            Ingredients.Add(new Turkey());
            Ingredients.Add(new Turkey());
            Ingredients.Add(new Lettuce());
            Ingredients.Add(new Lettuce());
            Ingredients.Add(new Mayonnaise());
            Ingredients.Add(new Bread());
            Ingredients.Add(new Turkey());
            Ingredients.Add(new Turkey());
            Ingredients.Add(new Lettuce());
            Ingredients.Add(new Lettuce());
            Ingredients.Add(new Mayonnaise());
            Ingredients.Add(new Bread());
        }
    }
}
