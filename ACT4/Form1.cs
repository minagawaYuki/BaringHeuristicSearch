using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

// SIMULATED ANNEALING

namespace ACT4
{
    public partial class Form1 : Form
    {
        int side;
        int n = 6;
        SixState startState;
        SixState currentState;
        int moveCounter;

        double temperature = 100.0;
        double coolingRate = 0.99;

        public Form1()
        {
            InitializeComponent();

            side = pictureBox1.Width / n;

            startState = randomSixState();
            currentState = new SixState(startState);

            updateUI();
            label1.Text = "Attacking pairs: " + getAttackingPairs(startState);
        }

        private void updateUI()
        {
            pictureBox2.Refresh();

            label3.Text = "Attacking pairs: " + getAttackingPairs(currentState);
            label4.Text = "Moves: " + moveCounter;

            listBox1.Items.Clear();
            label2.Text = "Temperature: " + temperature.ToString("F2");
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if ((i + j) % 2 == 0)
                    {
                        e.Graphics.FillRectangle(Brushes.Black, i * side, j * side, side, side);
                    }
                    if (j == currentState.Y[i])
                    {
                        e.Graphics.FillEllipse(Brushes.Fuchsia, i * side, j * side, side, side);
                    }
                }
            }
        }

        private SixState randomSixState()
        {
            Random r = new Random();
            return new SixState(r.Next(n), 
                                r.Next(n), 
                                r.Next(n), 
                                r.Next(n), 
                                r.Next(n), 
                                r.Next(n));
        }

        private int getAttackingPairs(SixState f)
        {
            int attackers = 0;

            for (int rf = 0; rf < n; rf++)
            {
                for (int tar = rf + 1; tar < n; tar++)
                {
                    if (f.Y[rf] == f.Y[tar]) 
                        attackers++;
                    if (f.Y[tar] == f.Y[rf] + tar - rf) 
                        attackers++;
                    if (f.Y[rf] == f.Y[tar] + tar - rf) 
                        attackers++;
                }
            }

            return attackers;
        }

        private SixState randomNeighbor(SixState state)
        {
            Random r = new Random();
            SixState neighbor = new SixState(state);
            int row = r.Next(n);
            int newY = r.Next(n);
            neighbor.Y[row] = newY;
            return neighbor;
        }

        private bool acceptMove(int currentAttackingPairs, int newAttackingPairs)
        {
            if (newAttackingPairs < currentAttackingPairs)
            {
                return true;
            }
            else
            {
                double acceptanceProbability = Math.Exp((currentAttackingPairs - newAttackingPairs) / temperature);
                Random r = new Random();
                return r.NextDouble() < acceptanceProbability;
            }
        }

        private void executeMove(SixState newState)
        {
            currentState = new SixState(newState);
            moveCounter++;
            updateUI();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (getAttackingPairs(currentState) > 0 && temperature > 1)
            {
                SixState neighbor = randomNeighbor(currentState);
                int currentAttackingPairs = getAttackingPairs(currentState);
                int neighborAttackingPairs = getAttackingPairs(neighbor);

                if (acceptMove(currentAttackingPairs, neighborAttackingPairs))
                {
                    executeMove(neighbor);
                }

                temperature *= coolingRate;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            startState = randomSixState();
            currentState = new SixState(startState);

            temperature = 100.0;
            moveCounter = 0;

            updateUI();
            pictureBox1.Refresh();
            label1.Text = "Attacking pairs: " + getAttackingPairs(startState);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            while (getAttackingPairs(currentState) > 0 && temperature > 1)
            {
                button1_Click(sender, e);
            }
        }
    }
}