Imports System.io
Module Module1

  Dim folder As DirectoryInfo = New DirectoryInfo(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Replace("file:\", ""))
  Sub Main()
    If Not Directory.Exists($"{folder.FullName}\output") Then
      Directory.CreateDirectory($"{folder.FullName}\output")
    End If
    Dim outputFolder As New DirectoryInfo($"{folder.FullName}\output")
    Dim files As List(Of FileInfo) = folder.GetFiles().Where(Function(x) x.Extension = ".mp4").ToList()
    Console.WriteLine($"{files.Count} file(s) will be scanned.")
    For Each file In files
      Try
        Console.ForegroundColor = ConsoleColor.Cyan
        Console.WriteLine($"Removing duplicated frames from {file.Name}")
        Console.ResetColor()
        RemoveDuplicatedFrames(file, outputFolder)
        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine($"{files.IndexOf(file) + 1}/{files.Count} - {file.Name} scan completed.")
        Console.ResetColor()
        If System.IO.File.Exists($"{outputFolder.FullName}\{file.Name}") Then
          'Arquivo criado com sucesso. Pode apagar o original
          DeleteFile(file, outputFolder)
        End If
      Catch ex As Exception
        Console.WriteLine(ex.Message)
      End Try
    Next

    Console.WriteLine("Operation completed! Press any key to exit...")
    Console.ReadKey()
  End Sub

  Private Sub DeleteFile(f As FileInfo, outputFolder As DirectoryInfo)
    If File.Exists($"{outputFolder.FullName}\{f.Name}") Then
      Try
        File.Delete(f.FullName)
        Console.ForegroundColor = ConsoleColor.Red
        Console.WriteLine($"Arquivo original {f.Name} excluído!")
        Console.ResetColor()
      Catch ex As Exception
        Console.ForegroundColor = ConsoleColor.Magenta
        Console.WriteLine($"Erro excluindo arquivo {f.Name}")
      End Try
    End If
  End Sub

  Private Sub RemoveDuplicatedFrames(file As FileInfo, outputFolder As DirectoryInfo)
    Dim processo As New Process()
    Dim config As New ProcessStartInfo()
    config.UseShellExecute = False
    config.RedirectStandardOutput = False
    config.FileName = "ffmpeg"
    config.Arguments = $" -i ""{file.Name}"" -vf mpdecimate,setpts=N/FRAME_RATE/TB ""{outputFolder.Name}\{file.Name}"""
    processo.StartInfo = config
    Console.ForegroundColor = ConsoleColor.Yellow
    Console.WriteLine("Startando processo de remoção de quadros duplicados.")
    Console.ResetColor()
    processo.Start()
    processo.WaitForExit()
    Console.ForegroundColor = ConsoleColor.DarkCyan
    processo.WaitForExit()
  End Sub
End Module
