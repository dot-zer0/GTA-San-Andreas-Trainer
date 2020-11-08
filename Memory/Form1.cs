using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Memory
{
    public partial class Form1 : Form
    {
        MemoryHack memory = new MemoryHack("gta_sa");

        #region Основные Адреса функции

        int iAddressMoney = 0xB7CE50;

        int iHealth = 0x10ABBDB0; 
        int iChangedHealth = 99999999; 

        int iAmmo = 0x7428B0;
        byte[] bAmmo = new byte[3] { 0x90, 0x90, 0x90 };

        int iAmmo2 = 0x7428E6;
        byte[] bAmmo2 = new byte[3] { 0x90, 0x90, 0x90 };

        int iRun = 0xB7CEE4;
        byte[] bRun = new byte[2] { 0x01, 0x00 };

        int iNoFireDamage = 0xB7CEE6;
        byte[] bNoFireDamage = new byte[4] { 0x01, 0x64, 0x64, 0x00 };

        int iGraffity = 0xA9AD74; // Кол-во закрашенных граффити

        int iRepaintCar = 0x96C009; // Бесплатное перекрашивание машины
        byte[] bRepaintCar = new byte[2] { 0x01, 0x00 };

        int iChaosMode = 0x969175; // Режим хаоса
        byte[] bChaosMode = new byte[2] { 0x01, 0x00 };

        #endregion

        #region Статистика игрока

        int iShapePlayer = 0xB793D4; // Фигура
        int iStamina = 0xB793D8; // Выносливость
        int iMusculature = 0xB793DC; // Мускалатура
        int iSexuality = 0xB793E4; // Сексуальность
        int iPistol = 0xB79496; // Навыки с пистолетом
        int iPistolSilence = 0xB79498; // Пистолет с глушителем
        int iShotgun = 0xB794A0; // Дробовик
        int iEdge = 0xB794A4; // Обрез
        int iUZI = 0xB794AC; // Узи
        int iAK47 = 0xB794B4; // AK-47
        int iM4 = 0xB794B8; // M4

        #endregion

        public Form1()
        {
            InitializeComponent();
            timer1.Start();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

            label1.Text = Process.GetProcessesByName("gta_sa").Any() ? "GTA: San Andreas Found!" : "Game not found";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Text = "Активировано";
            int iTextBox = Convert.ToInt32(textBox1.Text);
            memory.WriteInt32((IntPtr)iAddressMoney, iTextBox);
            Console.Beep();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Environment.Exit(1337);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = Process.GetProcessesByName("gta_sa").Any() ? "GTA: San Andreas Found!" : "Game not found";

            // Обновление значения с помощью таймера
            int iCJMoney = memory.ReadInt32((IntPtr)iAddressMoney);
            label2.Text = "Money: " + iCJMoney;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.Text = "Активировано";
            memory.WriteFloat((IntPtr)iHealth, (float)iChangedHealth);
            Console.Beep();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            button5.Text = "Активировано";
            memory.WriteByteArray((IntPtr)iAmmo, bAmmo);
            memory.WriteByteArray((IntPtr)iAmmo2, bAmmo2);
            Console.Beep();
        }


        private void button6_Click(object sender, EventArgs e)
        {
            button6.Text = "Активировано";
            memory.WriteByteArray((IntPtr)iRun, bRun);
            Console.Beep();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            button7.Text = "Активировано";
            memory.WriteByteArray((IntPtr)iNoFireDamage, bNoFireDamage);
            Console.Beep();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            memory.WriteFloat((IntPtr)iShapePlayer, (float)0);
            memory.WriteFloat((IntPtr)iStamina, (float)1000);
            memory.WriteFloat((IntPtr)iMusculature, (float)1000);
            memory.WriteFloat((IntPtr)iSexuality, (float)1000);
            memory.WriteFloat((IntPtr)iPistol, (float)1000);
            memory.WriteFloat((IntPtr)iPistolSilence, (float)1000);
            memory.WriteFloat((IntPtr)iShotgun, (float)1000);
            memory.WriteFloat((IntPtr)iEdge, (float)1000);
            memory.WriteFloat((IntPtr)iUZI, (float)1000);
            memory.WriteFloat((IntPtr)iAK47, (float)1000);
            memory.WriteFloat((IntPtr)iM4, (float)1000);

            button4.Text = "Активировано";
            Console.Beep();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            memory.WriteInt32((IntPtr)iGraffity, 100);
            button8.Text = "Активировано";
            Console.Beep();
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            memory.WriteByteArray((IntPtr)iRepaintCar, bRepaintCar);
            button9.Text = "Активировано";
            Console.Beep();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            memory.WriteByteArray((IntPtr)iChaosMode, bChaosMode);
            button10.Text = "Активировано";
            Console.Beep();
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }
    }
}
