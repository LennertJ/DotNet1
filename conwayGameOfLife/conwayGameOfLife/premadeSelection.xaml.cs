using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace conwayGameOfLife
{
    /// <summary>
    /// Interaction logic for premadeSelection.xaml
    /// </summary>
    public partial class premadeSelection : Window
    {
        private List<Patern> paternList = new List<Patern>();
        //private string saveFiles = "C:\\Users\\Lennert Jans\\source\\repos\\conwayGameOfLife\\paterns";
        private string previewFiles = "C:\\Users\\Lennert Jans\\source\\repos\\conwayGameOfLife\\previews";//moet aangepast worden naar de locatie van dit op uw pc

        private string saveFiles =     @"..\..\..\paterns";
        //private string previewFiles = @".\previews";


        DataTransfer transferDel;

        public premadeSelection(DataTransfer transferDel)
        {

            InitializeComponent();
            LoadFiles();
            for (int i = 0; i < paternList.Count; i++)
            {
                paternListBox.Items.Add(paternList[i].Name);
            }
            this.transferDel = transferDel;
        }

        private void LoadFiles()
        {
            string[] dirList = Directory.GetFiles(saveFiles, "*.txt");
            string[] imgList = Directory.GetFiles(previewFiles, "*.png");
            string name;
            for (int i =0; i < dirList.Length; i++) {
                string[] tmp = dirList[i].Split('\\');
                name = tmp[tmp.Length - 1].Substring(0, tmp[tmp.Length - 1].Length-4);
                paternList.Add(new Patern(40,dirList.ElementAt(i),imgList.ElementAt(i),name));
            }
        }

        private void paternListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int plaats = paternListBox.SelectedIndex;
            previewImage.Source = new BitmapImage(new Uri(paternList[plaats].FileName, UriKind.RelativeOrAbsolute));
            //previewImage.Source = new BitmapImage(Resources.);

        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {

            int plaats = paternListBox.SelectedIndex;
            transferDel.Invoke(paternList[plaats].LivingCells);
            this.Close();
        }
    }
}
