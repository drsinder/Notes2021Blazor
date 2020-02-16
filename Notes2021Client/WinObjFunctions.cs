using System.Windows.Forms;

namespace Notes2021Client
{
    public static class WinObjFunctions
    {
        public static int CountGridWidth(DataGridView dgv)
        {
            int width = 0;
            foreach (DataGridViewColumn column in dgv.Columns)
                if (column.Visible)
                    width += column.Width;
            return width + 20;
        }
    }
}
