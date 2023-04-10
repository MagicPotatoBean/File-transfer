Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Public Class Form1
    Private Sub SendFile(FileName As String)


        Try




            Dim Input As Boolean = False
            Dim IP As String = InputBox("Enter the IP address or hostname of your target", "IP Address", "127.0.0.1")
            Dim Port As Integer = InputBox("Enter the Port of your target", "Port", "13333")
            Dim Client As New TcpClient
            Client.Connect(IP, Port)
            MsgBox("Device connected")
            Input = True
            Dim NWStream As NetworkStream = Client.GetStream
            Dim bytesToSend(Client.SendBufferSize) As Byte
            Dim FI As New FileInfo(FileName)
            ProgressBar1.Maximum = FI.Length
            Dim FileSTR As New FileStream(FileName, FileMode.Open, FileAccess.Read)
            Dim FileReader As New BinaryReader(FileSTR)
            Dim numBytesRead As Integer
            Dim Ipos As Integer


            Do Until Ipos >= FI.Length
                numBytesRead = FileSTR.Read(bytesToSend, 0, bytesToSend.Length)
                NWStream.Write(bytesToSend, 0, numBytesRead)
                Ipos = Ipos + numBytesRead
                ProgressBar1.Value += numBytesRead
                ProgressBar1.Update()
                NWStream.Flush()
            Loop
            MsgBox("File Sent Successfully")
            NWStream.Flush()
            NWStream.Close()
            FileSTR.Close()
            FileReader.Close()
            Enabled = True
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub ReceiveFile(FileName As String)
        Try

            If File.Exists(FileName) Then
                File.Delete(FileName)
            End If

            Dim IP As String = GetIPv4Address()

            Dim TCP As New TcpListener(IPAddress.Any, InputBox("Enter port you would like to use.", "Port", "13333"))
            TCP.Start()


            Dim Client As TcpClient = TCP.AcceptTcpClient()
            Dim NWStream As NetworkStream = Client.GetStream
            Dim bytesToRead(Client.ReceiveBufferSize) As Byte
            Dim numBytesRead As Integer
            Dim BUFFER_SIZE As Integer = Client.ReceiveBufferSize
            Dim FileSTR As New FileStream(FileName, FileMode.Append, FileAccess.Write)
            Dim BinWrite As New BinaryWriter(FileSTR)
            Dim Bytes As New Integer
            Do
                numBytesRead = 0
                numBytesRead = NWStream.Read(bytesToRead, 0, BUFFER_SIZE)
                BinWrite.Write(bytesToRead, 0, numBytesRead)
                Bytes = Bytes + numBytesRead
            Loop Until numBytesRead = 0

            FileSTR.Close()
            NWStream.Close()
            Client.Close()
            MsgBox("File saved to [" & FileName & "]")
            Enabled = True
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        With SaveFileDialog1
            .Title = ""
            .ShowDialog()
        End With
        Dim ThreadReceive As New Thread(AddressOf ReceiveFile)
        ThreadReceive.Start(SaveFileDialog1.FileName)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        With OpenFileDialog1
            .CheckFileExists = True
            .Title = ""
            .ShowDialog()
        End With
        Dim ThreadReceive As New Thread(AddressOf SendFile)
        ThreadReceive.Start(OpenFileDialog1.FileName)
    End Sub

    Private Function GetIPv4Address() As String
        GetIPv4Address = String.Empty
        Dim strHostName As String = System.Net.Dns.GetHostName()
        Dim iphe As System.Net.IPHostEntry = System.Net.Dns.GetHostEntry(strHostName)

        For Each ipheal As System.Net.IPAddress In iphe.AddressList
            If ipheal.AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork Then
                GetIPv4Address = ipheal.ToString()
            End If
        Next

    End Function
End Class


