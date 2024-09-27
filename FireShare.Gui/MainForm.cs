using FluentFTP;
using System.IO;
using System.Web;

namespace FireShare
{
    public partial class MainForm : Form
    {
        private FtpClient? ftpClient;
        private string currentPath = "/usb1_1";
        private List<FtpListItem> ftpList = [];
        private string vlcPath = "C:\\Program Files\\VideoLAN\\VLC\\vlc.exe";

        public MainForm()
        {
            InitializeComponent();

            dataGridView1.Columns.Add("Item", "Item");

            Connect();
            GetCurrentListing();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Connect();
            GetCurrentListing();
        }

        private void Connect()
        {
            ftpClient = new FtpClient("192.168.1.1", "admin", "admin");
            ftpClient.AutoConnect();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var ftpItem = ftpList[e.RowIndex];

                if (ftpItem.Type == FtpObjectType.Directory)
                {
                    currentPath = ftpItem.FullName;
                    GetCurrentListing();
                }
                else if (ftpItem.Type == FtpObjectType.File)
                {
                    string fileUrl = HttpUtility.UrlPathEncode(ftpItem.FullName);

                    System.Diagnostics.Process.Start($"\"{vlcPath}\"", $"\"ftp://admin:admin@192.168.1.1/{fileUrl}\"");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var parts = currentPath.Split('/');
            string newPath = parts[parts.Length - 1];
            currentPath = currentPath.Replace($"/{newPath}", string.Empty);

            GetCurrentListing();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            GetCurrentListing();
        }

        private void GetCurrentListing()
        {
            if (CheckConnection())
            {
                ftpList.Clear();
                dataGridView1.Rows.Clear();
                foreach (FtpListItem item in ftpClient!.GetListing(currentPath))
                {
                    ftpList.Add(item);
                    dataGridView1.Rows.Add(item.Name);
                }
                textBox1.Text = currentPath;
            }
        }

        private bool CheckConnection()
        {
            string? errorMessage = null;
            if (ftpClient is null)
            {
                errorMessage = "FTP Client was never connected.";
            }
            else if (!ftpClient.IsConnected)
            {
                errorMessage = "FTP Client is not connected.";
            }
            else if (!ftpClient.IsAuthenticated)
            {
                errorMessage = "FTP Client is not authenticated.";
            }

            if (errorMessage is not null)
            {
                MessageBox.Show(
                    errorMessage,
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
