using System.Globalization;

namespace launchmaui.Utilities;

public class GreaterThanZeroConverter : IValueConverter
{
  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    return value is int i && i > 0;
  }

  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}

public class StringNotNullOrEmptyConverter : IValueConverter
{
  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    return !string.IsNullOrEmpty(value?.ToString());
  }

  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}

public class StringIsNullOrEmptyConverter : IValueConverter
{
  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    return string.IsNullOrEmpty(value?.ToString());
  }

  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}

public class ArrayIsNullOrEmptyConverter : IValueConverter
{
  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    return (value is object[] array && array.Length! > 0) || value is null;
  }

  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}

public class Converters
{
  public static IValueConverter StringNotNullOrEmpty => new StringNotNullOrEmptyConverter();
  public static IValueConverter StringIsNullOrEmpty => new StringIsNullOrEmptyConverter();
  public static IValueConverter GreaterThanZero => new GreaterThanZeroConverter();
  public static IValueConverter ArrayIsNullOrEmpty => new ArrayIsNullOrEmptyConverter();
}