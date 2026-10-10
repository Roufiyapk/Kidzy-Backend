namespace Kidzy.Application.Constants;

public static class ProductFilterConstants
{
    // AGE

    public static readonly string[] Ages =
    {
        "0–2 Years",
        "2–6 Years",
        "6-10 Years"
       
    };


    // GENDER

    public static readonly string[] Genders =
    {
        "Boys",
        "Girls"
    };


    // PRICE RANGE

    public static readonly string[] PriceRanges =
    {
        "Under500",
        "500To1000",
        "1000To2000",
        "Above2000"
    };


    // SORTING

    public static readonly string[] SortOptions =
    {
        "Relevance",
        "PriceLowToHigh",
        "PriceHighToLow"
    };

    public static readonly string[] StockStatuses =
{
    "InStock",
    "LowStock",
    "OutOfStock"
};
}