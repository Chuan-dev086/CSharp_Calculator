using System;
using System.Data;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MyDesktopApp;

public partial class MainWindow : Window
{
    private string expression = "";

    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnButtonClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            string content = button.Content?.ToString() ?? "";

            if (content == "C")
            {
                expression = "";
                DisplayText.Text = "0";
                return;
            }

            if (content == "DEL")
            {
                if (expression.Length > 0)
                    expression = expression.Substring(0, expression.Length - 1);
                DisplayText.Text = string.IsNullOrEmpty(expression) ? "0" : expression;
                return;
            }

            if (content == "=")
            {
                if (string.IsNullOrEmpty(expression))
                    return;
                try
                {
                    var result = new DataTable().Compute(expression, null);
                    expression = result.ToString() ?? "0";
                    DisplayText.Text = expression;
                }
                catch
                {
                    DisplayText.Text = "Error";
                    expression = "";
                }
                return;
            }

            // ==========================================
            // 阶段 A：科学运算（开根号、平方）
            // ==========================================
            if (content == "√" || content == "x²")
            {
                if (string.IsNullOrEmpty(expression))
                    return;
                try
                {
                    var currentResult = Convert.ToDouble(new DataTable().Compute(expression, null));
                    if (content == "√")
                        currentResult = Math.Sqrt(currentResult);
                    else if (content == "x²")
                        currentResult = Math.Pow(currentResult, 2);

                    expression = currentResult.ToString();
                    DisplayText.Text = expression;
                }
                catch
                {
                    DisplayText.Text = "Error";
                    expression = "";
                }
                return;
            }

            // ==========================================
            // 阶段 B：正负号与百分号
            // ==========================================
            if (content == "+/-")
            {
                if (string.IsNullOrEmpty(expression))
                    return;
                try
                {
                    // 简单处理：算出一个具体值，取负，放回屏幕
                    var val = Convert.ToDouble(new DataTable().Compute(expression, null));
                    expression = (-val).ToString();
                    DisplayText.Text = expression;
                }
                catch
                { /* 忽略错误 */
                }
                return;
            }

            if (content == "%")
            {
                if (string.IsNullOrEmpty(expression))
                    return;
                try
                {
                    // 算出一个具体值，除以100，放回屏幕
                    var val = Convert.ToDouble(new DataTable().Compute(expression, null));
                    expression = (val / 100.0).ToString();
                    DisplayText.Text = expression;
                }
                catch
                { /* 忽略错误 */
                }
                return;
            }

            // ==========================================
            // 常规输入
            // ==========================================
            if (
                string.IsNullOrEmpty(expression)
                && (content == "+" || content == "*" || content == "/" || content == ")")
            )
                return;

            if (expression.Length < 30)
            {
                expression += content;
                DisplayText.Text = expression;
            }
        }
    }
}
