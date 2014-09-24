using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GraphLibraryForCSharp;

namespace DemonstrationProject
{
    public partial class Demo : Form
    {
        #region Variables

        private List<Vertex> _bfsVertices = new List<Vertex>();
        private List<Vertex> _dfsVertices = new List<Vertex>();
        private List<Vertex> _dijkstrasVertices = new List<Vertex>();
        private IGraph _graph;

        private string _lastAlgorithm = "";
        private Graphics _pictureOfGraph;
        private List<Vertex> _primsVertices = new List<Vertex>();

        #endregion

        public Demo()
        {
            InitializeComponent();
            InitializeStyles();
        }

        #region PrivateMethods
        
        private void InitializeStyles()
        {
            txtVerticesSizeFont.Text = "22";
            pbVerticesName.BackColor = Color.Black;
            pbVerticesColor.BackColor = Color.FromArgb(255, 24, 101, 28);
            chbVerticesBold.Checked = true;

            pbEdgesColor.BackColor = Color.FromArgb(255, 88, 228, 115);

            txtWeightsSizeFont.Text = "16";
            pbWeightsColor.BackColor = Color.Blue;
            chbWeightsBold.Checked = false;

            txtVerticesWeightsSizeFont.Text = "18";
            pbVerticesWeightsColor.BackColor = Color.Red;
            chbVerticesWeightsBold.Checked = false;

            pbEdgeColor1.BackColor = Color.FromArgb(255, 88, 228, 115);
            pbEdgeColor1.Visible = true;
            pbEdgeColor2.Visible = true;
        }
        private void SetStyles()
        {
            var graphStyles = new GraphStyles();

            var fontSize = Convert.ToInt32(txtVerticesSizeFont.Text);

            graphStyles.VerticeNameFont = chbVerticesBold.Checked
                                              ? new Font("Times New Roman", fontSize, FontStyle.Bold)
                                              : new Font("Times New Roman", fontSize, FontStyle.Regular);

            graphStyles.VerticePointBrush = new SolidBrush(pbVerticesColor.BackColor);
            graphStyles.VerticeNameBrush = new SolidBrush(pbVerticesName.BackColor);

            graphStyles.EdgePen = new Pen(pbEdgesColor.BackColor, 1);

            fontSize = Convert.ToInt32(txtWeightsSizeFont.Text);

            graphStyles.WeightFont = chbWeightsBold.Checked
                                         ? new Font("Times New Roman", fontSize, FontStyle.Bold)
                                         : new Font("Times New Roman", fontSize, FontStyle.Regular);

            graphStyles.WeightBrush = new SolidBrush(pbWeightsColor.BackColor);

            fontSize = Convert.ToInt32(txtVerticesWeightsSizeFont.Text);

            graphStyles.VerticeWeightFont = chbVerticesWeightsBold.Checked
                                                ? new Font("Times New Roman", fontSize, FontStyle.Bold)
                                                : new Font("Times New Roman", fontSize, FontStyle.Regular);

            graphStyles.VerticeWeightBrush = new SolidBrush(pbVerticesWeightsColor.BackColor);

            graphStyles.EdgePenList = new List<Pen>();

            if (pbEdgeColor1.BackColor != Color.Transparent)
                graphStyles.EdgePenList.Add(new Pen(pbEdgeColor1.BackColor, 1));
            if (pbEdgeColor2.BackColor != Color.Transparent)
                graphStyles.EdgePenList.Add(new Pen(pbEdgeColor2.BackColor, 1));
            if (pbEdgeColor3.BackColor != Color.Transparent)
                graphStyles.EdgePenList.Add(new Pen(pbEdgeColor3.BackColor, 1));
            if (pbEdgeColor4.BackColor != Color.Transparent)
                graphStyles.EdgePenList.Add(new Pen(pbEdgeColor4.BackColor, 1));
            if (pbEdgeColor5.BackColor != Color.Transparent)
                graphStyles.EdgePenList.Add(new Pen(pbEdgeColor5.BackColor, 1));
            if (pbEdgeColor6.BackColor != Color.Transparent)
                graphStyles.EdgePenList.Add(new Pen(pbEdgeColor6.BackColor, 1));

            _graph.SetCustomStyles(graphStyles);
        }

        private void DrawGraph()
        {
            plDrawingWindow.Refresh();
            _pictureOfGraph = plDrawingWindow.CreateGraphics();

            try
            {
                _graph.DrawGraphOnCircle(_pictureOfGraph);
            }
            catch (ArgumentException exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private int[] CountingSize(OpenFileDialog file)
        {
            var fileReader = new StreamReader(file.OpenFile());

            var jIndex = 0;
            var iIndex = 1;

            var firstLine = fileReader.ReadLine();
            if (firstLine != null)
                jIndex = firstLine.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).GetLength(0);
            
            while ((fileReader.ReadLine()) != null) 
                iIndex++;
            
            fileReader.Close();

            return new[] {iIndex, jIndex};
        }
        private int[,] LoadFile(int countOfLines, int countOfLines2, OpenFileDialog file)
        {
            var fileReader = new StreamReader(file.OpenFile());
            var nrLine = 0;
            string line;

            var adjacencyMatrix = new int[countOfLines,countOfLines2];

            while ((line = fileReader.ReadLine()) != null)
            {
                var actualLine = line.Split(new[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);

                for (var i = 0; i < actualLine.Length; i++)
                    adjacencyMatrix[nrLine, i] = Convert.ToInt32(actualLine[i]);
                nrLine++;
            }
            return adjacencyMatrix;
        }

        private double[,] LoadWeights(int countOfLines, int countOfLines2, OpenFileDialog file)
        {
            var fileReader = new StreamReader(file.OpenFile());
            var nrLine = 0;
            string line;

            var matrixOfWeights = new double[countOfLines,countOfLines2];


            while ((line = fileReader.ReadLine()) != null)
            {
                var actualLine = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                for (var i = 0; i < actualLine.Length; i++)
                    matrixOfWeights[nrLine, i] = Convert.ToDouble(actualLine[i]);
                nrLine++;
            }
            return matrixOfWeights;
        }
       
        #endregion

        #region Editing

        private void btnLoadUndirectedGraph_Click(object sender, EventArgs e)
        {
            var file = new OpenFileDialog();

            if (file.ShowDialog() == DialogResult.OK)
            {
                var countOfLines = CountingSize(file);
                if (chbWeightedGraph.Checked)
                {
                    try
                    {
                        var matrixOfWeights = LoadWeights(countOfLines[0], countOfLines[1], file);

                        _graph = new UndirectedGraph(matrixOfWeights);
                        DrawGraph();
                        rtxtReportingWindow.Text += "Wczytano nowy graf nieskierowany ważony.\n";
                    }
                    catch (ArgumentException exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                    catch (FormatException exception)
                    {
                        MessageBox.Show("Macierz sąsiedztwa zawiera nieprawidłowe elementy."); 
                    }
                }
                else
                {
                    try
                    {
                        var adjacencyMatrix = LoadFile(countOfLines[0], countOfLines[1], file);
                    
                        _graph = new UndirectedGraph(adjacencyMatrix);
                        DrawGraph();
                        rtxtReportingWindow.Text += "Wczytano nowy graf nieskierowany.\n";
                    }
                    catch (ArgumentException exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                    catch (FormatException exception)
                    {
                        MessageBox.Show("Macierz sąsiedztwa zawiera nieprawidłowe elementy.");
                    }
                }
            }
        }

        private void btnLoadDirectedGraph_Click(object sender, EventArgs e)
        {
            var file = new OpenFileDialog();

            if (file.ShowDialog() == DialogResult.OK)
            {
                var countOfLines = CountingSize(file);

                if (chbWeightedGraph.Checked)
                {
                    try
                    {
                        var matrixOfWeights = LoadWeights(countOfLines[0], countOfLines[1], file);

                        _graph = new DirectedGraph(matrixOfWeights);
                        DrawGraph();
                        rtxtReportingWindow.Text += "Wczytano nowy graf skierowany ważony.\n";
                    }
                    catch (ArgumentException exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                    catch (FormatException exception)
                    {
                        MessageBox.Show("Macierz wag zawiera nieprawidłowe elementy.");
                    }
                }
                else
                {
                    try
                    {
                        var adjacencyMatrix = LoadFile(countOfLines[0], countOfLines[1], file);

                        _graph = new DirectedGraph(adjacencyMatrix);
                        DrawGraph();
                        rtxtReportingWindow.Text += "Wczytano nowy graf skierowany.\n";
                    }
                    catch (ArgumentException exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                    catch (FormatException exception)
                    {
                        MessageBox.Show("Macierz sąsiedztwa zawiera nieprawidłowe elementy.");
                    }
                }
            }
        }

        private void btnVertexAdd_Click(object sender, EventArgs e)
        {
            var vertexName = txtVertexName.Text.Trim(' ');
            var vertex = new Vertex(vertexName);

            try
            {
                _graph.AddVertex(vertex);
                DrawGraph();
                rtxtReportingWindow.Text += "Dodano nowy wierzchołek.\n";
            }
            catch (ArgumentException exception)
            {
                MessageBox.Show(exception.Message);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Błąd: graf nie istnieje.");
            }
        }

        private void btnVertexDel_Click(object sender, EventArgs e)
        {
            var vertexName = txtVertexName.Text.Trim(' ');
            var vertex = new Vertex(vertexName);

            try
            {
                _graph.RemoveVertex(vertex);
                DrawGraph();
                rtxtReportingWindow.Text += "Usunięto wierzchołek.\n";
            }
            catch (ArgumentException nullException)
            {
                MessageBox.Show(nullException.Message);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Błąd: graf nie istnieje.");
            }
        }

        private void btnEdgeAdd_Click(object sender, EventArgs e)
        {
            var vertexNameBegin = txtNameEdgeBegin.Text.Trim(' ');
            var vertexNameEnd = txtNameEdgeEnd.Text.Trim(' ');
            var weight = txtWeight.Text.Trim(' ');

            var vertexBegin = _graph.GetVertex(vertexNameBegin);
            var vertexEnd = _graph.GetVertex(vertexNameEnd);

            var edge = new Edge(vertexBegin, vertexEnd);

            if (_graph.IsWeighted && weight == "") MessageBox.Show("Błąd: graf ważony wymaga podania wagi krawędzi.");
            else
            {
                try
                {
                    _graph.AddEdge(edge);

                    if (weight != "") _graph.AddWeight(edge, Convert.ToDouble(weight));

                    DrawGraph();
                    rtxtReportingWindow.Text += "Dodano nową krawędź.\n";
                }
                catch (NullReferenceException)
                {
                    MessageBox.Show("Błąd: graf nie istnieje.");
                }
                catch (ArgumentException exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }

        private void btnEdgeDel_Click(object sender, EventArgs e)
        {
            var vertexNameBegin = txtNameEdgeBegin.Text.Trim(' ');
            var vertexNameEnd = txtNameEdgeEnd.Text.Trim(' ');

            var vertexBegin = _graph.GetVertex(vertexNameBegin);
            var vertexEnd = _graph.GetVertex(vertexNameEnd);

            var edge = _graph.GetEdge(vertexBegin, vertexEnd);

            try
            {
                _graph.RemoveEdge(edge);
                DrawGraph();
                rtxtReportingWindow.Text += "Usunięto krawędź.\n";
            }
            catch (ArgumentException nullException)
            {
                MessageBox.Show(nullException.Message);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Błąd: graf nie istnieje.");
            }
        }

        private void btnWeightsLoad_Click(object sender, EventArgs e)
        {
            var plik = new OpenFileDialog();

            if (plik.ShowDialog() == DialogResult.OK)
            {
                var countOfLines = CountingSize(plik);
                var weights = LoadWeights(countOfLines[0], countOfLines[1], plik);

                try
                {
                    _graph.AddAllWeights(weights);
                    DrawGraph();
                    rtxtReportingWindow.Text += "Dodano wagi.\n";
                }
                catch (ArgumentException exception)
                {
                    MessageBox.Show(exception.Message);
                }
                catch (NullReferenceException)
                {
                    MessageBox.Show("Błąd: graf nie istnieje.");
                }
            }
        }

        private void btnWeightsDel_Click(object sender, EventArgs e)
        {
            try
            {
                _graph.RemoveAllWeights();

                DrawGraph();
                rtxtReportingWindow.Text += "Usunięto wagi.\n";
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Błąd: graf nie istnieje.");
            }
        }

        private void btnWeightEdit_Click(object sender, EventArgs e)
        {
            try
            {
                var vertexNameBegin = txtNameEdgeBegin.Text.Trim(' ');
                var vertexNameEnd = txtNameEdgeEnd.Text.Trim(' ');

                var vertexBegin = _graph.GetVertex(vertexNameBegin);
                var vertexEnd = _graph.GetVertex(vertexNameEnd);

                var edge = _graph.GetEdge(vertexBegin, vertexEnd);
                var weight = Convert.ToDouble(txtWeight.Text.Trim(' '));

                _graph.AddWeight(edge, weight);
                DrawGraph();
                rtxtReportingWindow.Text += "Waga została zmnieniona.\n";
            }
            catch (ArgumentException nullException)
            {
                MessageBox.Show(nullException.Message);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Błąd: graf nie istnieje.");
            }
            catch (FormatException)
            {
                MessageBox.Show("Błąd: wpisz wartość wagi.");
            }
        }

        #endregion

        #region Drawing

        private void btnEdgesDefault_Click(object sender, EventArgs e)
        {
            pbEdgesColor.BackColor = Color.FromArgb(255, 88, 228, 115);
        }

        private void btnWeightsDefault_Click(object sender, EventArgs e)
        {
            txtWeightsSizeFont.Text = "16";
            pbWeightsColor.BackColor = Color.Blue;
            chbWeightsBold.Checked = false;
        }

        private void btnVerticesWeightsDefault_Click(object sender, EventArgs e)
        {
            txtVerticesWeightsSizeFont.Text = "18";
            pbVerticesWeightsColor.BackColor = Color.Red;
            chbVerticesWeightsBold.Checked = false;
        }

        private void btnVerticesDefault_Click(object sender, EventArgs e)
        {
            txtVerticesSizeFont.Text = "22";
            pbVerticesName.BackColor = Color.Black;
            pbVerticesColor.BackColor = Color.Black;
            chbVerticesBold.Checked = true;
        }

        private void pbVerticesName_Click(object sender, EventArgs e)
        {
            if (cdCheckColor.ShowDialog() == DialogResult.OK)
            {
                pbVerticesName.BackColor = cdCheckColor.Color;
            }
        }

        private void pbVerticesColor_Click(object sender, EventArgs e)
        {
            if (cdCheckColor.ShowDialog() == DialogResult.OK)
            {
                pbVerticesColor.BackColor = cdCheckColor.Color;
            }
        }

        private void pbEdgesColor_Click(object sender, EventArgs e)
        {
            if (cdCheckColor.ShowDialog() == DialogResult.OK)
            {
                pbEdgesColor.BackColor = cdCheckColor.Color;
            }
        }

        private void pbWeightsColor_Click(object sender, EventArgs e)
        {
            if (cdCheckColor.ShowDialog() == DialogResult.OK)
            {
                pbWeightsColor.BackColor = cdCheckColor.Color;
            }
        }

        private void pbVerticesWeightsColor_Click(object sender, EventArgs e)
        {
            if (cdCheckColor.ShowDialog() == DialogResult.OK)
            {
                pbVerticesWeightsColor.BackColor = cdCheckColor.Color;
            }
        }

        private void btnListOfColorsDefault_Click(object sender, EventArgs e)
        {
            pbEdgeColor1.BackColor = Color.FromArgb(255, 88, 228, 115);
            pbEdgeColor2.BackColor = Color.Transparent;
            pbEdgeColor3.BackColor = Color.Transparent;
            pbEdgeColor4.BackColor = Color.Transparent;
            pbEdgeColor5.BackColor = Color.Transparent;
            pbEdgeColor6.BackColor = Color.Transparent;
            pbEdgeColor1.Visible = true;
            pbEdgeColor2.Visible = true;
            pbEdgeColor3.Visible = false;
            pbEdgeColor4.Visible = false;
            pbEdgeColor5.Visible = false;
            pbEdgeColor6.Visible = false;
        }

        private void pbEdgeColor1_Click(object sender, EventArgs e)
        {
            if (cdCheckColor.ShowDialog() == DialogResult.OK)
            {
                pbEdgeColor1.BackColor = cdCheckColor.Color;
            }
        }

        private void pbEdgeColor2_Click(object sender, EventArgs e)
        {
            if (cdCheckColor.ShowDialog() == DialogResult.OK)
            {
                pbEdgeColor2.BackColor = cdCheckColor.Color;
            }
        }

        private void pbEdgeColor3_Click(object sender, EventArgs e)
        {
            if (cdCheckColor.ShowDialog() == DialogResult.OK)
            {
                pbEdgeColor3.BackColor = cdCheckColor.Color;
            }
        }

        private void pbEdgeColor4_Click(object sender, EventArgs e)
        {
            if (cdCheckColor.ShowDialog() == DialogResult.OK)
            {
                pbEdgeColor4.BackColor = cdCheckColor.Color;
            }
        }

        private void pbEdgeColor5_Click(object sender, EventArgs e)
        {
            if (cdCheckColor.ShowDialog() == DialogResult.OK)
            {
                pbEdgeColor5.BackColor = cdCheckColor.Color;
            }
        }

        private void pbEdgeColor6_Click(object sender, EventArgs e)
        {
            if (cdCheckColor.ShowDialog() == DialogResult.OK)
            {
                pbEdgeColor6.BackColor = cdCheckColor.Color;
            }
        }

        private void AddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cdCheckColor.ShowDialog() == DialogResult.OK)
            {
                var owner = DeleteToolStripMenuItem.Owner as ContextMenuStrip;

                if (owner != null) owner.SourceControl.BackColor = cdCheckColor.Color;
            }
        }

        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var owner = DeleteToolStripMenuItem.Owner as ContextMenuStrip;
            var source = owner.SourceControl;

            switch (source.Name)
            {
                case "pbEdgeColor1":
                    if (pbEdgeColor2.BackColor == Color.Transparent)
                    {
                        MessageBox.Show("Błąd: nie można usunąć - do listy kolorów musi zostać dodany przynajmniej jeden kolor.");
                        break;
                    }
                    pbEdgeColor1.BackColor = pbEdgeColor2.BackColor;
                    pbEdgeColor2.BackColor = pbEdgeColor3.BackColor;
                    pbEdgeColor3.BackColor = pbEdgeColor4.BackColor;
                    pbEdgeColor4.BackColor = pbEdgeColor5.BackColor;
                    pbEdgeColor5.BackColor = pbEdgeColor6.BackColor;
                    pbEdgeColor6.BackColor = Color.Transparent;
                    break;
                case "pbEdgeColor2":
                    pbEdgeColor2.BackColor = pbEdgeColor3.BackColor;
                    pbEdgeColor3.BackColor = pbEdgeColor4.BackColor;
                    pbEdgeColor4.BackColor = pbEdgeColor5.BackColor;
                    pbEdgeColor5.BackColor = pbEdgeColor6.BackColor;
                    pbEdgeColor6.BackColor = Color.Transparent;
                    break;
                case "pbEdgeColor3":
                    pbEdgeColor3.BackColor = pbEdgeColor4.BackColor;
                    pbEdgeColor4.BackColor = pbEdgeColor5.BackColor;
                    pbEdgeColor5.BackColor = pbEdgeColor6.BackColor;
                    pbEdgeColor6.BackColor = Color.Transparent;
                    break;
                case "pbEdgeColor4":
                    pbEdgeColor4.BackColor = pbEdgeColor5.BackColor;
                    pbEdgeColor5.BackColor = pbEdgeColor6.BackColor;
                    pbEdgeColor6.BackColor = Color.Transparent;
                    break;
                case "pbEdgeColor5":
                    pbEdgeColor5.BackColor = pbEdgeColor6.BackColor;
                    pbEdgeColor6.BackColor = Color.Transparent;
                    break;
                case "pbEdgeColor6":
                    pbEdgeColor6.BackColor = Color.Transparent;
                    break;
            }
        }

        private void pbEdgeColor1_BackColorChanged(object sender, EventArgs e)
        {
            pbEdgeColor1.Visible = true;
            pbEdgeColor2.Visible = true;
        }

        private void pbEdgeColor2_BackColorChanged(object sender, EventArgs e)
        {
            if (pbEdgeColor1.BackColor == Color.Transparent)
            {
                pbEdgeColor2.Visible = false;
            }
            else if (pbEdgeColor2.BackColor == Color.Transparent)
            {
                pbEdgeColor2.Visible = true;
                pbEdgeColor3.Visible = false;
            }
            else
            {
                pbEdgeColor2.Visible = true;
                pbEdgeColor3.Visible = true;
            }
        }

        private void pbEdgeColor3_BackColorChanged(object sender, EventArgs e)
        {
            if (pbEdgeColor2.BackColor == Color.Transparent)
            {
                pbEdgeColor3.Visible = false;
            }
            else if (pbEdgeColor3.BackColor == Color.Transparent)
            {
                pbEdgeColor3.Visible = true;
                pbEdgeColor4.Visible = false;
            }
            else
            {
                pbEdgeColor3.Visible = true;
                pbEdgeColor4.Visible = true;
            }
        }

        private void pbEdgeColor4_BackColorChanged(object sender, EventArgs e)
        {
            if (pbEdgeColor3.BackColor == Color.Transparent)
            {
                pbEdgeColor4.Visible = false;
            }
            else if (pbEdgeColor4.BackColor == Color.Transparent)
            {
                pbEdgeColor4.Visible = true;
                pbEdgeColor5.Visible = false;
            }
            else
            {
                pbEdgeColor4.Visible = true;
                pbEdgeColor5.Visible = true;
            }
        }

        private void pbEdgeColor5_BackColorChanged(object sender, EventArgs e)
        {
            if (pbEdgeColor4.BackColor == Color.Transparent)
            {
                pbEdgeColor5.Visible = false;
            }
            else if (pbEdgeColor5.BackColor == Color.Transparent)
            {
                pbEdgeColor5.Visible = true;
                pbEdgeColor6.Visible = false;
            }
            else
            {
                pbEdgeColor5.Visible = true;
                pbEdgeColor6.Visible = true;
            }
        }

        private void pbEdgeColor6_BackColorChanged(object sender, EventArgs e)
        {
            pbEdgeColor6.Visible = pbEdgeColor5.BackColor != Color.Transparent;
        }

        private void btnDrawGraph_Click(object sender, EventArgs e)
        {
            try
            {
                SetStyles();
                DrawGraph();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Błąd: nie można wykonać - graf nie istnieje.");
            }
        }

        private void btnDrawLastAlgorithmResult_Click(object sender, EventArgs e)
        {
                switch (_lastAlgorithm)
                {
                    case "bfs":
                        btnBfsShowResult_Click(sender, e);
                        break;
                    case "dfs":
                        btnShowDfsResult_Click(sender, e);
                        break;
                    case "dijkstra":
                        btnShowDijkstrasResult_Click(sender, e);
                        break;
                    case "prim":
                        btnShowPrimResult_Click(sender, e);
                        break;
                }      
        }

        #endregion

        #region Algorithms

        private void btnBfsAlgorithm_Click(object sender, EventArgs e)
        {
            try
            {
                var vertexName = txtBfsBeginVertex.Text.Trim(' ');
                var vertex = _graph.GetVertex(vertexName);
                _bfsVertices = _graph.BfsAlgorithm(vertex);

                if (_bfsVertices != null && _bfsVertices.Count > 0)
                {
                    _lastAlgorithm = "bfs";
                    MessageBox.Show(" Algorytm BFS wykonał się poprawnie.");
                    rtxtReportingWindow.Text += "Algorytm BFS wykonał się poprawnie.\n";
                    rtxtReportingWindow.Text += "Wynik działania algorytmu BFS:\n";
                    foreach (var item in _bfsVertices)
                    {
                        rtxtReportingWindow.Text += item.Name;
                        var distance = item.Distance == int.MaxValue ? "∞" : item.Distance.ToString();

                        if (item.Parent != null) rtxtReportingWindow.Text += ", \t nazwa rodzica: " + item.Parent.Name;
                        else rtxtReportingWindow.Text += ",\t nazwa rodzica: -";
                        rtxtReportingWindow.Text += ", \t odległość: " + distance + "\n";
                    }
                }
                else
                {
                    MessageBox.Show("Błąd: algorytm BFS nie wykonał się poprawnie (nie otrzymano wyniku).");
                    rtxtReportingWindow.Text += "Algorytm BFS nie wykonał się poprawnie (nie otrzymano wyniku).\n";
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Błąd: nie można wykonać - graf nie istnieje.");
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Błąd: wpisz nazwę wierzchołka.");
            }
        }

        private void btnBfsShowResult_Click(object sender, EventArgs e)
        {
            try
            {
                SetStyles();
                plDrawingWindow.Refresh();
                _pictureOfGraph = plDrawingWindow.CreateGraphics();
                _graph.ShowBfsResult(_bfsVertices, _pictureOfGraph, 2000);
                rtxtReportingWindow.Text += "Wyświetlono kompletne drzewo przeszukiwania BFS.\n";
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Błąd: nie można wykonać - graf nie istnieje.");
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Błąd: nie można wyświetlić wyniku - nie wykonano algorytmu BFS lub wynik nie zawiera żadnych krawędzi!");
                DrawGraph();
            }
        }

        private void btnDfsAlgorithm_Click(object sender, EventArgs e)
        {
            try
            {
                var vertexName = txtDfsBeginVertex.Text.Trim(' ');
                var vertex = _graph.GetVertex(vertexName);
                _dfsVertices = _graph.DfsAlgorithm(vertex);
                if (_dfsVertices != null)
                {
                    _lastAlgorithm = "dfs";
                    MessageBox.Show("Algorytm DFS wykonał się poprawnie.");
                    rtxtReportingWindow.Text += "Algorytm DFS wykonał się poprawnie.\n";
                    rtxtReportingWindow.Text += "Wynik działania algorytmu DFS:\n";

                    foreach (var item in _dfsVertices)
                    {
                        rtxtReportingWindow.Text += item.Name;
                        if (item.Parent != null) rtxtReportingWindow.Text += ", \t nazwa rodzica: " + item.Parent.Name;
                        else rtxtReportingWindow.Text += ", \t nazwa rodzica: -";
                        rtxtReportingWindow.Text += ", \t czas odwiedzenia: " + item.VisitTime +
                                                    ", \t czas przetworzenia: " + item.ProcessTime + "\n";
                    }
                }
                else
                {
                    MessageBox.Show("Błąd: algorytm DFS nie wykonał się poprawnie (nie otrzymano wyniku).");
                    rtxtReportingWindow.Text += "Algorytm DFS nie wykonał się poprawnie (nie otrzymano wyniku).\n";
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Błąd: nie można wykonać - graf nie istnieje.");
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Błąd: wpisz nazwę wierzchołka.");
            }
        }

        private void btnShowDfsResult_Click(object sender, EventArgs e)
        {
            try
            {
                SetStyles();
                plDrawingWindow.Refresh();
                _pictureOfGraph = plDrawingWindow.CreateGraphics();
                _graph.ShowDfsResult(_dfsVertices, _pictureOfGraph, 2000);
                rtxtReportingWindow.Text += "Wyświetlono kompletny las przeszukiwania DFS.\n";
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Błąd: nie można wykonać - graf nie istnieje.");
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Błąd: nie można wyświetlić wyniku - nie wykonano algorytmu DFS lub wynik nie zawiera żadnych krawędzi.");
                DrawGraph();
            }
        }

        private void btnDijkstrasAlgorithm_Click(object sender, EventArgs e)
        {
            try
            {
                var v1 = txtDijkstraBeginVertex.Text.Trim(' ');
                var vertex = _graph.GetVertex(v1);
                _dijkstrasVertices = _graph.DijkstrasAlgorithm(vertex);

                if (_dijkstrasVertices != null)
                {
                    _lastAlgorithm = "dijkstra";
                    MessageBox.Show(" Algorytm Dijkstry wykonał się poprawnie.");
                    rtxtReportingWindow.Text += "Algorytm Dijkstry wykonał się poprawnie.\n";
                    rtxtReportingWindow.Text += "Wynik działania algorytmu Dijkstry:\n";

                    foreach (Vertex item in _dijkstrasVertices)
                    {
                        rtxtReportingWindow.Text += item.Name;
                        if (item.Parent != null) rtxtReportingWindow.Text += ", \t nazwa rodzica: " + item.Parent.Name;
                        else rtxtReportingWindow.Text += ",\t nazwa rodzica: -";
                        rtxtReportingWindow.Text += ", \t odległość: " + item.Distance + "\n";
                    }
                }
                else
                {
                    MessageBox.Show("Błąd: algorytm Dijkstry nie wykonał się poprawnie (nie otrzymano wyniku).");
                    rtxtReportingWindow.Text += "Algorytm Dijkstry nie wykonał się poprawnie (nie otrzymano wyniku).\n";
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Błąd: nie można wykonać - graf nie istnieje.");
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Błąd: wpisz nazwę wierzchołka.");
            }
        }

        private void btnShowDijkstrasResult_Click(object sender, EventArgs e)
        {
            try
            {
                SetStyles();
                plDrawingWindow.Refresh();
                _pictureOfGraph = plDrawingWindow.CreateGraphics();
                _graph.ShowDijkstrasResult(_dijkstrasVertices, _pictureOfGraph, 2000);
                rtxtReportingWindow.Text += "Wyświetlono kompletny wynik algorytmu Dijkstry.\n";
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Błąd: nie można wykonać - graf nie istnieje.");
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Błąd: nie można wyświetlić wyniku - nie wykonano algorytmu Dijkstry lub wynik nie zawiera żadnych krawędzi.");
            }
        }

        private void btnPrimsAlgorithm_Click(object sender, EventArgs e)
        {
            try
            {
                var vertexName = txtPrimBeginVertex.Text.Trim(' ');
                var vertex = _graph.GetVertex(vertexName);
                _primsVertices = _graph.PrimsAlgorithm(vertex);

                if (_primsVertices != null)
                {
                    _lastAlgorithm = "prim";
                    MessageBox.Show("Algorytm Prima wykonał się poprawnie.");
                    rtxtReportingWindow.Text += "Algorytm Prima wykonał się poprawnie.\n";
                    rtxtReportingWindow.Text += "Wynik działania algorytmu Prima:\n";

                    foreach (var item in _primsVertices)
                    {
                        rtxtReportingWindow.Text += item.Name;
                        if (item.Parent != null) rtxtReportingWindow.Text += ", \t nazwa rodzica: " + item.Parent.Name;
                        else rtxtReportingWindow.Text += ",\t nazwa rodzica: -";
                        rtxtReportingWindow.Text += ", \t klucz: " + item.Key + "\n";
                    }
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Błąd: nie można wykonać - graf nie istnieje.");
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Błąd: wpisz nazwę wierzchołka.");
                DrawGraph();
            }
        }

        private void btnShowPrimResult_Click(object sender, EventArgs e)
        {
            try
            {
                SetStyles();
                plDrawingWindow.Refresh();
                _pictureOfGraph = plDrawingWindow.CreateGraphics();
                _graph.ShowPrimsResult(_primsVertices, _pictureOfGraph, 2000);
                rtxtReportingWindow.Text += "Wyświetlono kompletny wynik algorytmu Prima.\n";
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Błąd: nie można wykonać - graf nie istnieje.");
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Błąd: nie można wyświetlić wyniku - nie wykonano algorytmu Prima lub wynik nie zawiera żadnych krawędzi.");
                DrawGraph();
            }
        }

        #endregion

        #region KeyPressAndTextChanged

        private void rtxtReportingWindow_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void rtxtReportingWindow_TextChanged(object sender, EventArgs e)
        {
            rtxtReportingWindow.SelectionStart = rtxtReportingWindow.Text.Length;
            rtxtReportingWindow.ScrollToCaret();
        }

        private void txtWeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) && e.KeyChar!=',' && e.KeyChar.ToString()!= char.ConvertFromUtf32(8) )
            {
                e.Handled = true;
            }
        }

        private void txtVertexName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) &&  !char.IsLetter(e.KeyChar) && e.KeyChar != '_' && e.KeyChar != '-' && e.KeyChar.ToString() != char.ConvertFromUtf32(8))
            {
                e.Handled = true;
            }
        }

        private void txtNameEdgeBegin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != '_' && e.KeyChar != '-' && e.KeyChar.ToString() != char.ConvertFromUtf32(8))
            {
                e.Handled = true;
            }
        }

        private void txtNameEdgeEnd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != '_' && e.KeyChar != '-' && e.KeyChar.ToString() != char.ConvertFromUtf32(8))
            {
                e.Handled = true;
            }
        }

        private void txtBfsBeginVertex_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != '_' && e.KeyChar != '-' && e.KeyChar.ToString() != char.ConvertFromUtf32(8))
            {
                e.Handled = true;
            }
        }

        private void txtDfsBeginVertex_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != '_' && e.KeyChar != '-' && e.KeyChar.ToString() != char.ConvertFromUtf32(8))
            {
                e.Handled = true;
            }
        }

        private void txtDijkstraBeginVertex_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != '_' && e.KeyChar != '-' && e.KeyChar.ToString() != char.ConvertFromUtf32(8))
            {
                e.Handled = true;
            }
        }

        private void txtPrimBeginVertex_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != '_' && e.KeyChar != '-' && e.KeyChar.ToString() != char.ConvertFromUtf32(8))
            {
                e.Handled = true;
            }
        }

        private void txtVerticesSizeFont_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) && e.KeyChar.ToString() != char.ConvertFromUtf32(8))
            {
                e.Handled = true;
            }
        }

        private void txtWeightsSizeFont_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) && e.KeyChar.ToString() != char.ConvertFromUtf32(8))
            {
                e.Handled = true;
            }
        }

        private void txtVerticesWeightsSizeFont_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) && e.KeyChar.ToString() != char.ConvertFromUtf32(8))
            {
                e.Handled = true;
            }
        }

        #endregion

        
    }
}