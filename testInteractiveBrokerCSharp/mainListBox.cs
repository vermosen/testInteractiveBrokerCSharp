using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MySql.Data.MySqlClient;                               // mysql linkage
using Krs.Ats.IBNet;                                        // include IBNet

namespace testInteractiveBrokerCSharp
{
    public partial class mainForm : Form
    {

        // members
        static tickStreamManager    manager_    = null;     // the tick manager
        static IBClient             client_     = null;     // the IB client
        static MySqlConnection      mySqlConn_  = null;     // mysql connection
        static List<String>         stockList_   = null;    // list of stock to listen

        // manager_.Run() delegate
        static void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            manager_.Run();
        }

        public mainForm()
        {
            InitializeComponent();

            // print welcome message
            this.mainListBox.Items.Add("Welcome to the Interactive Broker test Application");
            this.mainListBox.Items.Add("--------------------------------------------------");
            this.mainListBox.Items.Add("");
            this.mainListBox.Items.Add("Please, push the start button...");
            this.mainListBox.Items.Add("");
        }

        private void mainTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void startButton_Click(object sender, EventArgs e)
        {

            // step 1: trying to open database
            this.mainListBox.Items.Add("Step 1: Attempting to open database connection...");

            try
            {
                mySqlConn_.Open();                              // Opens the connection to the database
                this.mainListBox.Items.Add("Database connection has been opened succesfully !");

            }
            catch (Exception exp)
            {
                this.mainListBox.Items.Add("An error occured: " + exp.Message);
                return;                                         // on error, exit
            }

            // step 2: trying to open IB connection
            this.mainListBox.Items.Add("Step 2: Attempting to open interactive broker connection...");

            if (mySqlConn_.State == ConnectionState.Open) {

                try
                {
                    client_.Connect("localhost", 7496, 2);
                    this.mainListBox.Items.Add("Connection to TWS has been opened succesfully !");
                }
                catch (Exception exp)
                {
                    this.mainListBox.Items.Add("An error occured: " + exp.Message);
                    return;                                         // on error, exit
                }

            }

            //Step 4: Initialize the TickMain object
            manager_        = new tickStreamManager(client_, stockList_, mySqlConn_);
            manager_.doGet  = true;

            //Setup a worker to call main.Run() asycronously.
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += new DoWorkEventHandler(bg_DoWork);
            bg.RunWorkerAsync();
           
            //Chill until the user hits enter then stop the TickMain object
            //Console.ReadLine();
            //manager_.doGet = false;
            //disconnect
            //client.Disconnect();
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            // initializing variables
            client_     = new IBClient();
            mySqlConn_  = new MySqlConnection(
                "server=mac;"
              + "DATABASE=tick_db;"
              + "USER=admin;"
              + "PASSWORD=test01");
            
            stockList_  = new List<string>();
            stockList_.Add("AAPL"   );                              // Apple
            stockList_.Add("SPY"    );                              // SPY ETF
            stockList_.Add("F"      );                              // Ford
            stockList_.Add("DIA"    );                              // dow jones industrial

        }

        private void mainListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
