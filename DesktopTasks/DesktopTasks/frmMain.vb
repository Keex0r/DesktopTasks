Imports System.ComponentModel
Imports System.Runtime.InteropServices

Public Class frmMain
#Region "Fields"
    Dim bds As New BindingSource
    Dim WithEvents Tasks As New System.ComponentModel.BindingList(Of DeskTask)
#End Region
#Region "Interop"
    Public Const SPI_SETDESKWALLPAPER As Integer = 20
    Public Const SPIF_UPDATEINIFILE As Integer = 1
    Public Const SPIF_SENDCHANGE As Integer = 2

    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Public Shared Function SystemParametersInfo(uAction As Integer, uParam As Integer, lpvParam As String, fuWinIni As Integer) As Integer
    End Function
#End Region
#Region "Constructor"
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        DateTimePicker2.Format = DateTimePickerFormat.Time
        DateTimePicker2.ShowUpDown = False
        Tasks = New System.ComponentModel.BindingList(Of DeskTask)
        bds.DataSource = Tasks
        DataGridView1.DataSource = bds
        LoadTasks()
        Timer1.Start()
        UpdateWallpaper()
    End Sub
#End Region
#Region "Set Wallpaper"
    Private Sub UpdateWallpaper()
        Dim sSize As Size = Screen.PrimaryScreen.Bounds.Size
        Using bmp As New Bitmap(sSize.Width, sSize.Height)
            Dim f As New Font("Lucida Bright", 40)
            Using g As Graphics = Graphics.FromImage(bmp)
                g.Clear(Color.Black)
                'Draw update time
                Dim update = Date.Now.ToString
                Dim fUp As New Font("Lucida Bright", 12)
                Dim sUp = g.MeasureString(update, fUp)
                g.DrawString(update, fUp, Brushes.White, CInt(1920 - sUp.Width - 5), 1)

                Dim thistasks = (From t In Tasks Where t.Date > Date.Now Select t Order By t.Date).ToList
                Dim sizes As New List(Of SizeF)
                Dim texts As New List(Of String)

                If thistasks.Count > 0 Then
                    'Create text entries
                    For Each t In thistasks
                        Dim remaining = (t.Date - Date.Now)
                        Dim ttext As String = ""
                        If remaining.TotalDays > 1 Then
                            ttext = CInt(remaining.TotalDays).ToString & " d"
                        ElseIf remaining.TotalHours > 1 Then
                            ttext = CInt(remaining.TotalHours).ToString & " h"
                        ElseIf remaining.TotalMinutes > 1 Then
                            ttext = CInt(remaining.TotalMinutes).ToString & " m"
                        Else
                            ttext = CInt(remaining.TotalSeconds).ToString & " s"
                        End If
                        Dim text = ttext & ": " & t.Description
                        texts.Add(text)
                        sizes.Add(g.MeasureString(text, f))
                    Next
                Else
                    texts.Add("All tasks done! :-)")
                    sizes.Add(g.MeasureString("All tasks done! :-)", f))
                End If

                'Draw text entries
                Dim y As Integer = 30
                Dim i As Integer = 0
                For Each t In texts
                    Dim x As Integer = CInt(1920 - 30 - sizes(i).Width)
                    g.DrawString(texts(i), f, Brushes.White, x, y)
                    y += CInt(sizes(i).Height + 15)
                    i += 1
                Next
            End Using
            'Set wallpaper
            bmp.Save(IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TaskBG.png"), System.Drawing.Imaging.ImageFormat.Png)
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TaskBG.png"), SPIF_UPDATEINIFILE Or SPIF_SENDCHANGE)
        End Using
    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        UpdateWallpaper()
    End Sub
#End Region
#Region "Add task"
    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        If String.IsNullOrEmpty(tbTask.Text) Then Exit Sub
        Dim t As New DeskTask
        t.Date = DateTimePicker1.Value
        Dim otherdate = DateTimePicker2.Value
        t.Date = New Date(t.Date.Year, t.Date.Month, t.Date.Day, otherdate.Hour, otherdate.Minute, otherdate.Second)
        t.Description = tbTask.Text
        Tasks.Add(t)
        Save()
        tbTask.Text = ""
        UpdateWallpaper()
    End Sub
#End Region
#Region "IO"
    Private Sub Save()
        Dim savetasks As New List(Of DeskTask)
        savetasks.AddRange(Tasks)
        Dim ser As New System.Xml.Serialization.XmlSerializer(GetType(List(Of DeskTask)))
        Using sb As New System.IO.StringWriter
            ser.Serialize(sb, savetasks)
            My.Settings.Tasks = sb.ToString
            My.Settings.Save()
        End Using
    End Sub
    Private Sub LoadTasks()
        Timer1.Stop()
        Tasks.Clear()
        Try
            Dim ser As New System.Xml.Serialization.XmlSerializer(GetType(List(Of DeskTask)))
            Using sb As New System.IO.StringReader(My.Settings.Tasks)
                Dim newtasks = CType(ser.Deserialize(sb), List(Of DeskTask))
                For Each t In newtasks
                    Tasks.Add(t)
                Next
            End Using
        Catch ex As Exception
        End Try
        Timer1.Start()

    End Sub
#End Region
#Region "Other events"
    Private Sub DataGridView1_UserDeletedRow(sender As Object, e As DataGridViewRowEventArgs) Handles DataGridView1.UserDeletedRow
        Save()
    End Sub

    Private Sub NotifyIcon1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseDoubleClick
        Me.WindowState = FormWindowState.Normal
    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        If Me.WindowState = FormWindowState.Minimized Then
            Me.ShowInTaskbar = False
        Else
            Me.ShowInTaskbar = True
        End If
    End Sub

    Private Sub numDelay_ValueChanged(sender As Object, e As EventArgs) Handles numDelay.ValueChanged
        Timer1.Interval = Math.Min(Math.Max(1, CInt(numDelay.Value)), 3600) * 1000
    End Sub

    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        'Blank screen when program shuts down
        Dim sSize As Size = Screen.PrimaryScreen.Bounds.Size
        Using bmp As New Bitmap(sSize.Width, sSize.Height)
            Dim f As New Font("Lucida Bright", 40)
            Using g As Graphics = Graphics.FromImage(bmp)
                g.Clear(Color.Black)
            End Using
            bmp.Save(IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TaskBG.png"), System.Drawing.Imaging.ImageFormat.Png)
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TaskBG.png"), SPIF_UPDATEINIFILE Or SPIF_SENDCHANGE)
        End Using
    End Sub
#End Region

End Class
