using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Data;
using System.Collections.ObjectModel; // 引入动态集合

namespace MyDesktopApp;

public partial class MainWindow : Window
{
    private string expression = "";
    
    // 这就是我们的“历史记录动态数组”，只要往里面加东西，界面就会自动刷新！
    private ObservableCollection<string> historyList = new ObservableCollection<string>();

    public MainWindow()
    {
        InitializeComponent();
        
        // 把右边的 ListBox 和我们的集合绑定起来
        HistoryListBox.ItemsSource = historyList;
    }

    private void OnButtonClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            string content = button.Content?.ToString() ?? "";

            // 清空
            if (content == "C")
            {
                expression = "";
                DisplayText.Text = "0";
                return;
            }

            // 退格
            if (content == "DEL")
            {
                if (expression.Length > 0)
                    expression = expression.Substring(0, expression.Length - 1);
                DisplayText.Text = string.IsNullOrEmpty(expression) ? "0" : expression;
                return;
            }

            // 等于号 (核心改动：计算完毕后，把记录加到右边)
            if (content == "=")
            {
                if (string.IsNullOrEmpty(expression)) return;
                try
                {
                    var result = new DataTable().Compute(expression, null);
                    string resultStr = result.ToString() ?? "0";
                    
                    // 把完整算式和结果加到历史记录里
                    historyList.Insert(0, $"{expression} = {resultStr}"); // Insert(0) 让最新的记录永远在最上面
                    
                    expression = resultStr;
                    DisplayText.Text = expression;
                }
                catch
                {
                    DisplayText.Text = "Error";
                    expression = "";
                }
                return;
            }

            // 科学运算（√ 和 x²）
            if (content == "√" || content == "x²")
            {
                if (string.IsNullOrEmpty(expression)) return;
                try
                {
                    var currentResult = Convert.ToDouble(new DataTable().Compute(expression, null));
                    if (content == "√") currentResult = Math.Sqrt(currentResult);
                    else if (content == "x²") currentResult = Math.Pow(currentResult, 2);

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

            // 正负号与百分号
            if (content == "+/-")
            {
                if (string.IsNullOrEmpty(expression)) return;
                try
                {
                    var val = Convert.ToDouble(new DataTable().Compute(expression, null));
                    expression = (-val).ToString();
                    DisplayText.Text = expression;
                } catch { }
                return;
            }

            if (content == "%")
            {
                if (string.IsNullOrEmpty(expression)) return;
                try
                {
                    var val = Convert.ToDouble(new DataTable().Compute(expression, null));
                    expression = (val / 100.0).ToString();
                    DisplayText.Text = expression;
                } catch { }
                return;
            }

            // 常规输入
            if (string.IsNullOrEmpty(expression) && (content == "+" || content == "*" || content == "/" || content == ")"))
                return;

            if (expression.Length < 30)
            {
                expression += content;
                DisplayText.Text = expression;
            }
        }
    }
}