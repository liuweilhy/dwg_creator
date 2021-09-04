using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace DwgCreator
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region 私有变量
        private string sourcePath = string.Empty;
        private string targetPath = string.Empty;
        private string sourceFolder = string.Empty;
        private DataTable dt;
        #endregion

        #region 公共属性
        public string SourcePath
        {
            get => sourcePath;
            set
            {
                sourcePath = value;
                NotifyPropertyChanged();
            }
        }

        public string TargetPath
        {
            get => targetPath;
            set
            {
                targetPath = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region 数据绑定

        /// <summary>
        /// 数据绑定事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// This method is called by the Set accessor of each property. The CallerMemberName attribute that is applied to the optional propertyName parameter causes the property name of the caller to be substituted as an argument.  
        /// </summary>
        /// <param name="propertyName"></param>
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog()
            {
                Title = "打开Excel表格",
                IsFolderPicker = false,
                Multiselect = false,
                EnsureFileExists = true,
                AddToMostRecentlyUsedList = true
            };
            dialog.Filters.Add(new CommonFileDialogFilter("Excel工作表", "*.xlsx"));

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SourcePath = dialog.FileName;
                sourceFolder = System.IO.Path.GetDirectoryName(sourcePath);
                // 读取Excel表格
                dt = ExcelHelper.ExcelToDataTable(sourcePath, 0, 0, true);
                dataGrid.ItemsSource = dt.DefaultView;
            }
        }

        private void ButtonSaveAs_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog()
            {
                Title = "保存dwg到目录",
                IsFolderPicker = true,
                Multiselect = false,
                EnsurePathExists = true,
                AddToMostRecentlyUsedList = true
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                TargetPath = dialog.FileName;
            }
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 目标文件夹是否存在
                string targetPath = this.targetPath;
                if (!Directory.Exists(targetPath))
                {
                    if (String.IsNullOrWhiteSpace(targetPath))
                        targetPath = sourceFolder;
                    else
                        Directory.CreateDirectory(targetPath);
                }

                // 生成dwg文件
                CreateDwgFiles(dt, targetPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// 生成dwg文件
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="folder">目标文件夹</param>
        private void CreateDwgFiles(DataTable dt, string folder)
        {
            if (dt is null || !Directory.Exists(folder))
                return;
            foreach (DataRow row in dt.Rows)
            {
                string fileName = row[1] as string;
                string shape = row[2] as string;
                double v1 = Convert.ToDouble(row[3]);
                double v2 = Convert.ToDouble(row[4]);
                double v3 = Convert.ToDouble(row[5]);
            }
        }

    }
}
