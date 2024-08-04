namespace ShoppingOnline.Interfaces
{
    public interface ICommon
    {
        string FormatNumber(object value, int SoSauDauPhay = 2);
        string HtmlRate(int rate);
        bool IsNumeric(object value);
    }
}
