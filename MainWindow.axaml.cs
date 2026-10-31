using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Data; // 引入这个命名空间，用来做表达式计算

namespace MyDesktopApp;

public partial class MainWindow : Window
{
    // 现在的逻辑：用一个字符串记录整个算式（比如 "1+2*(3+4)"）
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

            // 1. 如果点的是清空 (C)
            if (content == "C")
            {
                expression = "";
                DisplayText.Text = "0";
                return;
            }

            // 2. 如果点的是退格 (DEL)
            if (content == "DEL")
            {
                if (expression.Length > 0)
                {
                    // 砍掉最后一个字符
                    expression = expression.Substring(0, expression.Length - 1);
                }

                // 如果删光了，显示 0
                DisplayText.Text = string.IsNullOrEmpty(expression) ? "0" : expression;
                return;
            }

            // 3. 如果点的是等号 (=)
            if (content == "=")
            {
                if (string.IsNullOrEmpty(expression)) return;

                try
                {
                    // 使用 C# 的神器 DataTable 来自动计算带括号的算式
                    var result = new DataTable().Compute(expression, null);

                    // 把结果转为字符串（防止除以 0 产生无限小数）
                    expression = result.ToString() ?? "0";
                    DisplayText.Text = expression;
                }
                catch (Exception)
                {
                    // 比如你输入了 1++2 这种错误格式，会报错
                    DisplayText.Text = "Error";
                    expression = "";
                }
                return;
            }

            // 4. 其他情况：数字、小数点、加减乘除、左右括号，统统拼接到算式里
            // 为了防止用户刚打开就输入运算符报错
            if (string.IsNullOrEmpty(expression) && (content == "+" || content == "*" || content == "/" || content == ")"))
            {
                return;
            }

            // 限制输入长度，防止超出屏幕
            if (expression.Length < 25)
            {
                expression += content;
                DisplayText.Text = expression;
            }
        }
    }
}