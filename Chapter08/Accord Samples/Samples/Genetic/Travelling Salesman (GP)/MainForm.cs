// Traveling Salesman Problem using Genetic Algorithms
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2006-2011
// contacts@aforgenet.com
//

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;

using Accord.Genetic;
using Accord.Controls;
using Accord;
using ReflectSoftware.Insight;

namespace SampleApp
{

    public class MainForm : System.Windows.Forms.Form
    {
        private System.Windows.Forms.GroupBox groupBox1;
        private Accord.Controls.Chart mapControl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox citiesCountBox;
        private System.Windows.Forms.Button generateMapButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox selectionBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox iterationsBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox currentIterationBox;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        private int citiesCount = 20;
        private int populationSize = 40;
        private int iterations = 100;
        private int selectionMethod = 0;
        private bool greedyCrossover = true;

        private double[,] map = null;

        private Thread workerThread = null;
        private volatile bool needToStop = false;

        // Constructor
        public MainForm()
        {
            InitializeComponent();

            mapControl.RangeX = new Range(0, 1000);
            mapControl.RangeY = new Range(0, 1000);
            mapControl.AddDataSeries("map", Color.Red, Chart.SeriesType.Dots, 5, false);
            mapControl.AddDataSeries("path", Color.Blue, Chart.SeriesType.Line, 1, false);


            selectionBox.SelectedIndex = selectionMethod;
            UpdateSettings();
            GenerateMap();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.stopButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.generateMapButton = new System.Windows.Forms.Button();
            this.iterationsBox = new System.Windows.Forms.TextBox();
            this.startButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.citiesCountBox = new System.Windows.Forms.TextBox();
            this.selectionBox = new System.Windows.Forms.ComboBox();
            this.currentIterationBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.mapControl = new Accord.Controls.Chart();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.stopButton);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.generateMapButton);
            this.groupBox1.Controls.Add(this.iterationsBox);
            this.groupBox1.Controls.Add(this.startButton);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.citiesCountBox);
            this.groupBox1.Controls.Add(this.selectionBox);
            this.groupBox1.Controls.Add(this.currentIterationBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.mapControl);
            this.groupBox1.Location = new System.Drawing.Point(2, 4);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Size = new System.Drawing.Size(790, 554);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Todays Sales Route";
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(628, 446);
            this.stopButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(138, 28);
            this.stopButton.TabIndex = 4;
            this.stopButton.Text = "S&top";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(689, 182);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 18);
            this.label5.TabIndex = 6;
            this.label5.Text = "( 0 - inifinity )";
            // 
            // generateMapButton
            // 
            this.generateMapButton.Location = new System.Drawing.Point(666, 46);
            this.generateMapButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.generateMapButton.Name = "generateMapButton";
            this.generateMapButton.Size = new System.Drawing.Size(100, 26);
            this.generateMapButton.TabIndex = 3;
            this.generateMapButton.Text = "&Generate";
            this.generateMapButton.Click += new System.EventHandler(this.generateMapButton_Click);
            // 
            // iterationsBox
            // 
            this.iterationsBox.Location = new System.Drawing.Point(700, 154);
            this.iterationsBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.iterationsBox.Name = "iterationsBox";
            this.iterationsBox.Size = new System.Drawing.Size(69, 22);
            this.iterationsBox.TabIndex = 5;
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(628, 413);
            this.startButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(138, 28);
            this.startButton.TabIndex = 3;
            this.startButton.Text = "&Calculate Route";
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(626, 156);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 19);
            this.label4.TabIndex = 4;
            this.label4.Text = "Iterations:";
            // 
            // citiesCountBox
            // 
            this.citiesCountBox.Location = new System.Drawing.Point(734, 22);
            this.citiesCountBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.citiesCountBox.Name = "citiesCountBox";
            this.citiesCountBox.Size = new System.Drawing.Size(34, 22);
            this.citiesCountBox.TabIndex = 2;
            // 
            // selectionBox
            // 
            this.selectionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectionBox.Items.AddRange(new object[] {
            "Elite",
            "Rank",
            "Roulette"});
            this.selectionBox.Location = new System.Drawing.Point(628, 118);
            this.selectionBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.selectionBox.Name = "selectionBox";
            this.selectionBox.Size = new System.Drawing.Size(140, 24);
            this.selectionBox.TabIndex = 3;
            // 
            // currentIterationBox
            // 
            this.currentIterationBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.currentIterationBox.Location = new System.Drawing.Point(692, 520);
            this.currentIterationBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.currentIterationBox.Name = "currentIterationBox";
            this.currentIterationBox.ReadOnly = true;
            this.currentIterationBox.Size = new System.Drawing.Size(66, 15);
            this.currentIterationBox.TabIndex = 1;
            this.currentIterationBox.Text = "0";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(626, 96);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(134, 19);
            this.label3.TabIndex = 2;
            this.label3.Text = "Selection method:";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(626, 24);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "Houses to Visit:";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(626, 520);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(66, 20);
            this.label6.TabIndex = 0;
            this.label6.Text = "Iteration:";
            // 
            // mapControl
            // 
            this.mapControl.Location = new System.Drawing.Point(14, 24);
            this.mapControl.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.mapControl.Name = "mapControl";
            this.mapControl.Size = new System.Drawing.Size(592, 516);
            this.mapControl.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(799, 547);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Encyclopedias and Neurons";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        // Delegates to enable async calls for setting controls properties
        private delegate void SetTextCallback(System.Windows.Forms.Control control, string text);

        // Thread safe updating of control's text property
        private void SetText(System.Windows.Forms.Control control, string text)
        {
            if (control.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                Invoke(d, new object[] { control, text });
            }
            else
            {
                control.Text = text;
            }
        }

        // On main form closing
        private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // check if worker thread is running
            if ((workerThread != null) && (workerThread.IsAlive))
            {
                needToStop = true;
                while (!workerThread.Join(100))
                    Application.DoEvents();
            }
        }

        // Update settings controls
        private void UpdateSettings()
        {
            citiesCountBox.Text = citiesCount.ToString();
            iterationsBox.Text = iterations.ToString();
        }

        // Delegates to enable async calls for setting controls properties
        private delegate void EnableCallback(bool enable);

        // Enable/disale controls (safe for threading)
        private void EnableControls(bool enable)
        {
            if (InvokeRequired)
            {
                EnableCallback d = new EnableCallback(EnableControls);
                Invoke(d, new object[] { enable });
            }
            else
            {
                citiesCountBox.Enabled = enable;
                iterationsBox.Enabled = enable;
                selectionBox.Enabled = enable;

                generateMapButton.Enabled = enable;

                startButton.Enabled = enable;
                stopButton.Enabled = !enable;
            }
        }

        // Generate new map for the Traivaling Salesman problem
        private void GenerateMap()
        {
            Random rand = new Random((int)DateTime.Now.Ticks);

            // create coordinates array
            map = new double[citiesCount, 2];

            for (int i = 0; i < citiesCount; i++)
            {
                map[i, 0] = rand.Next(1001);
                map[i, 1] = rand.Next(1001);
            }

           
            // set the map
            mapControl?.UpdateDataSeries("map", map);
            // erase path if it is
            mapControl?.UpdateDataSeries("path", null);
        }

        // On "Generate" button click - generate map
        private void generateMapButton_Click(object sender, System.EventArgs e)
        {
            // get cities count
            try
            {
                //citiesCount = Math.Max(5, Math.Min(50, int.Parse(citiesCountBox.Text)));
                citiesCount = int.Parse(citiesCountBox.Text);
            }
            catch
            {
                citiesCount = 20;
            }
            citiesCountBox.Text = citiesCount.ToString();

            // regenerate map
            GenerateMap();
        }

        // On "Start" button click
        private void startButton_Click(object sender, System.EventArgs e)
        {
            populationSize = 40;

            try
            {
                iterations = Math.Max(0, int.Parse(iterationsBox?.Text));
            }
            catch
            {
                iterations = 100;
            }
            
            UpdateSettings();

            selectionMethod = selectionBox.SelectedIndex;
            greedyCrossover = true;

            // disable all settings controls except "Stop" button
            EnableControls(false);

            // run worker thread
            needToStop = false;
            workerThread = new Thread(SearchSolution);
            workerThread.Start();
        }

        // On "Stop" button click
        private void stopButton_Click(object sender, System.EventArgs e)
        {
            // stop worker thread
            if (workerThread != null)
            {
                needToStop = true;
                while (!workerThread.Join(100))
                    Application.DoEvents();
                workerThread = null;
            }
        }

        // Worker thread
        void SearchSolution()
        {
            TSPFitnessFunction fitnessFunction = new TSPFitnessFunction(map);
            Population population = new Population(populationSize, new TSPChromosome(map), fitnessFunction, 
                (selectionMethod == 0) ? new EliteSelection() : (selectionMethod == 1) ? new RankSelection() 
                : (ISelectionMethod)new RouletteWheelSelection());

            int i = 1;
            double[,] path = new double[citiesCount + 1, 2];

            while (!needToStop)
            {
                RILogManager.Default?.SendDebug("Running Epoch " + i);
                population.RunEpoch();
                ushort[] bestValue = ((PermutationChromosome)population.BestChromosome)?.Value;
                
                for (int j = 0; j < citiesCount; j++)
                {
                    if (bestValue != null && map != null)
                    {
                        path[j, 0] = map[bestValue[j], 0];
                        path[j, 1] = map[bestValue[j], 1];
                    }
                }
                if (bestValue != null && map != null)
                {
                    path[citiesCount, 0] = map[bestValue[0], 0];
                    path[citiesCount, 1] = map[bestValue[0], 1];
                }

                mapControl?.UpdateDataSeries("path", path);
                SetText(currentIterationBox, i.ToString("N0"));

                i++;
                if ((iterations != 0) && (i > iterations))
                    break;
            }

            EnableControls(true);
        }
    }
}
