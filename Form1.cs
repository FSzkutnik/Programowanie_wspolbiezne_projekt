using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Programowanie_wspolbiezne_projekt
{
    public partial class Form1 : Form
    {
        //dane z zadania startowe
        int MAKS_KIBICOW_NA_STADIONIE = 20; //mozna podyfikowac w gui
        const int LICZBA_BRAM = 3;
        const int MAKS_W_BRAMCE = 3;
        const int MAX_SKIP = 5;

        class Kibic //klasa z danymi kibica
        {
            public int Numer { get; set; }
            public string Druzyna { get; set; }
        }

        class Gate //klasa z danymi bramek na stadionie 
        {
            public int Id { get; }
            public int Count { get; set; }
            public string TeamOwner { get; set; }
            public SemaphoreSlim Lock { get; } = new SemaphoreSlim(1, 1);
            public SemaphoreSlim SlotSemaphore { get; }
            public Panel UiPanel { get; set; }
            public List<Kibic> Fans { get; } = new List<Kibic>();

            public Gate(int id, int capacity)
            {
                Id = id;
                Count = 0;
                TeamOwner = null;
                SlotSemaphore = new SemaphoreSlim(capacity, capacity); //semafor bramki
            }
        }

        List<Kibic> waitingQueue = new List<Kibic>(); //kolejka na stadion
        List<Kibic> notAdmitted = new List<Kibic>();  //lista nie wpuszczonych
        SemaphoreSlim queueLock = new SemaphoreSlim(1, 1); //semafor dla kolejki
        SemaphoreSlim placeLock = new SemaphoreSlim(1, 1); //semafor dla stadionu
        Gate[] gates = new Gate[LICZBA_BRAM];
        int availablePlaces; //liczba dostępnych miejsc na stadionie
        bool started = false; //flaga rozpoczecia symulacji

        public Form1()
        {
            InitializeComponent(); //tworzenie komponentów UI
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeBrames(); 
        }

        private void InitializeBrames() //Tworzenie bramek
        {
            panelGates.Controls.Clear();

            for (int i = 0; i < LICZBA_BRAM; i++)
            {
                gates[i] = new Gate(i + 1, MAKS_W_BRAMCE);
                var panel = new Panel
                {
                    BorderStyle = BorderStyle.FixedSingle,
                    Width = 100,
                    Height = 100,
                    Left = 10,
                    Top = i * 110 + 10
                };
                var lbl = new Label
                {
                    Text = $"Bramka {i + 1}",
                    Dock = DockStyle.Top,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                panel.Controls.Add(lbl);
                panelGates.Controls.Add(panel);
                gates[i].UiPanel = panel;
            }
        }

        private void BtnStart_Click(object sender, EventArgs e) //przycisk start
        {
            if (!int.TryParse(txtCapacity.Text.Trim(), out MAKS_KIBICOW_NA_STADIONIE) || MAKS_KIBICOW_NA_STADIONIE <= 0)
            {
                MessageBox.Show("Podaj poprawną liczbę miejsc.");
                return;
            }

            btnStart.Enabled = false;
            txtCapacity.Enabled = false;
            btnRestart.Enabled = true;

            availablePlaces = MAKS_KIBICOW_NA_STADIONIE;
            labelStadium.Text = $"Wolnych miejsc: {availablePlaces}";
            started = true;

            var lista = LoadQueue("kolejka.txt"); //wczytanie danych z pliku

            queueLock.Wait();
            waitingQueue.Clear();
            waitingQueue.AddRange(lista);
            notAdmitted.Clear();
            queueLock.Release();

            UpdateQueueDisplay();
            UpdateNotAdmittedDisplay();

            foreach (var kibic in lista)
            {
                var t = new Thread(WatekKibic) { IsBackground = true };
                t.Start(kibic);
            }
        }

        private void BtnRestart_Click(object sender, EventArgs e) //przycisk restart
        {
            Application.Restart();
        }

        List<Kibic> LoadQueue(string sciezka) //funkcja do wcyztania kolejki z pliku
        {
            var list = new List<Kibic>();
            int num = 1;
            foreach (var line in File.ReadLines(sciezka))
            {
                var txt = line.Trim();
                if (txt == "RM" || txt == "LV")
                    list.Add(new Kibic { Numer = num++, Druzyna = txt });
            }
            return list;
        }

        void UpdateQueueDisplay() //wysiwetlanie kolejki
        {
            if (InvokeRequired) { BeginInvoke(new Action(UpdateQueueDisplay)); return; }
            List<Kibic> snapshot;
            queueLock.Wait(); snapshot = new List<Kibic>(waitingQueue); queueLock.Release();

            panelQueue.Controls.Clear();
            int x = panelQueue.Width - 35;
            foreach (var k in snapshot)
            {
                var pic = new Panel
                {
                    Width = 30,
                    Height = 30,
                    BackColor = k.Druzyna == "LV" ? Color.Red : Color.White,
                    Left = x,
                    Top = (panelQueue.Height - 30) / 2,
                    BorderStyle = BorderStyle.FixedSingle
                };
                pic.Controls.Add(new Label
                {
                    Text = k.Numer.ToString(),
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 8, FontStyle.Bold),
                    ForeColor = k.Druzyna == "LV" ? Color.White : Color.Black,
                    BackColor = Color.Transparent
                });
                panelQueue.Controls.Add(pic);
                x -= 35;
            }
        }

        void UpdateNotAdmittedDisplay() //odrzuceni kibice 
        {
            if (InvokeRequired) { BeginInvoke(new Action(UpdateNotAdmittedDisplay)); return; }

            var panel = notAdmittedPanel;
            panel.Controls.Clear();
            int x = 5;

            foreach (var fan in notAdmitted)
            {
                var p = new Panel
                {
                    Width = 30,
                    Height = 30,
                    Left = x,
                    Top = (panel.Height - 30) / 2,
                    BackColor = fan.Druzyna == "LV" ? Color.Red : Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };
                p.Controls.Add(new Label
                {
                    Text = fan.Numer.ToString(),
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 8, FontStyle.Bold),
                    ForeColor = fan.Druzyna == "LV" ? Color.White : Color.Black,
                    BackColor = Color.Transparent
                });
                panel.Controls.Add(p);
                x += 35;
            }
        }

        void UpdateGateDisplay(Gate g) //aktualizacja bramek na gui
        {
            if (g.UiPanel.InvokeRequired)
            {
                g.UiPanel.Invoke(new Action<Gate>(UpdateGateDisplay), g);
                return;
            }
            for (int i = g.UiPanel.Controls.Count - 1; i >= 1; i--)
                g.UiPanel.Controls.RemoveAt(i);

            int x = 0;
            foreach (var fan in g.Fans)
            {
                var pic = new Panel
                {
                    Width = 30,
                    Height = 30,
                    BackColor = fan.Druzyna == "LV" ? Color.Red : Color.White,
                    Left = x,
                    Top = 30,
                    BorderStyle = BorderStyle.FixedSingle
                };
                pic.Controls.Add(new Label
                {
                    Text = fan.Numer.ToString(),
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 8, FontStyle.Bold),
                    ForeColor = fan.Druzyna == "LV" ? Color.White : Color.Black,
                    BackColor = Color.Transparent
                });
                g.UiPanel.Controls.Add(pic);
                x += 35;
            }
        }

        void UpdateStadiumDisplay() //zmiana ilosci wolnych miejsc na stadionie
        {
            if (InvokeRequired) { Invoke(new Action(UpdateStadiumDisplay)); return; }
            labelStadium.Text = $"Wolnych miejsc: {availablePlaces}";
        }

        void WatekKibic(object obj) //Kibic i jego zachowanie
        {
            var kibic = (Kibic)obj;
            Gate assignedGate = null;
            int skipCount = 0;
            var skipped = new HashSet<int>();

            while (true)
            {
                queueLock.Wait(); //sprawdzanie czy pierwszy w kolejce
                bool isFirst = waitingQueue.Count > 0 && waitingQueue[0].Numer == kibic.Numer;
                queueLock.Release();

                if (isFirst) //pierwszy
                {
                    foreach (var g in gates) //szukamy bramki dla kibica
                    {
                        if (g.SlotSemaphore.Wait(0))
                        {
                            g.Lock.Wait();
                            if (g.Count == 0 || g.TeamOwner == kibic.Druzyna)
                            {
                                g.Count++;
                                g.TeamOwner = kibic.Druzyna;
                                g.Fans.Add(kibic);
                                assignedGate = g;
                            }
                            g.Lock.Release();
                            if (assignedGate != null) break;
                            g.SlotSemaphore.Release();
                        }
                    }

                    if (assignedGate != null) //jeśli znaleźliśmy bramkę to zwalniamy miejsce w kolejce
                    {
                        queueLock.Wait();
                        waitingQueue.RemoveAt(0);
                        queueLock.Release();
                        UpdateQueueDisplay();
                        UpdateGateDisplay(assignedGate);
                        break;
                    }

                    if (skipCount < MAX_SKIP) //mechanizm przepuszczania 
                    {
                        Kibic next = null;
                        queueLock.Wait(); if (waitingQueue.Count > 1) next = waitingQueue[1]; queueLock.Release();
                        if (next != null && next.Druzyna != kibic.Druzyna && !skipped.Contains(next.Numer)) //Sprawdzamy, czy drugi w kolejce jest z innej drużyny i nie był jeszcze przeskoczony
                        {
                            bool nextCan = false;
                            foreach (var g in gates) //sprawdzamy czy warto się zamieniać czy kibic za nami wejdzie 
                            {
                                if (g.SlotSemaphore.CurrentCount > 0)
                                {
                                    g.Lock.Wait();
                                    bool ok = (g.Count == 0 || g.TeamOwner == next.Druzyna);
                                    g.Lock.Release();
                                    if (ok) { nextCan = true; break; }
                                }
                            }
                            if (nextCan)
                            {
                                queueLock.Wait();
                                if (waitingQueue[0].Numer == kibic.Numer && waitingQueue[1].Numer == next.Numer) //sprawdzamy czy nic się nie zmieniło i zamieniamy kibiców 
                                {
                                    waitingQueue[1] = waitingQueue[0];
                                    waitingQueue[0] = next;
                                    skipCount++;
                                    skipped.Add(next.Numer);
                                    UpdateQueueDisplay();
                                }
                                queueLock.Release();
                                Thread.Sleep(150);
                                continue;
                            }
                        }
                    }
                }
                Thread.Sleep(150);
            }

            Thread.Sleep(new Random().Next(1500, 3000)); //kontrola na bramkach
            assignedGate.Lock.Wait();  //mechanizm wychodzenia z bramek
            assignedGate.Count--;
            assignedGate.Fans.Remove(kibic);
            if (assignedGate.Count == 0) assignedGate.TeamOwner = null;
            assignedGate.Lock.Release();
            assignedGate.SlotSemaphore.Release();
            UpdateGateDisplay(assignedGate);

            placeLock.Wait(); //wchodzenie na stadion jesli jest miejsce 
            if (availablePlaces > 0)
            {
                availablePlaces--;
                placeLock.Release();
                UpdateStadiumDisplay();
            }
            else //jesli nie to odchodzimy
            {
                placeLock.Release();
                queueLock.Wait();
                notAdmitted.Add(kibic);
                queueLock.Release();
                UpdateNotAdmittedDisplay();
            }
        }
    }
}
