Imports System.Threading

Public Class Form1
    Public Delegate Sub Delegate1(ByVal myString As String) '自定义一个委托
    Dim ff As New Delegate1(AddressOf Compare)
    Public myThread As New Thread(AddressOf doWork)

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox1.Text = TextBox2.Text Then
            MsgBox("输入框一致")
        End If
    End Sub
    Public Shared Current As Form1

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Current = Me
        myThread.Start()

        Timer1.Enabled = True
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        'If TextBox1.Text = TextBox2.Text Then
        '    Timer1.Enabled = False
        '    MsgBox("输入框一致")
        '    Timer1.Enabled = True
        'End If
    End Sub

    Private Sub Compare(ByVal s)
        Try
            If StrComp(TextBox1.Text, "", CompareMethod.Text) <> 0 And StrComp(TextBox1.Text, TextBox2.Text, CompareMethod.Text) = 0 Then
                MsgBox(TextBox1.Text + "输入框一致")
            End If
        Catch ex As Exception

        End Try

    End Sub

    Sub doWork()
        While (True)
            Thread.Sleep(5000)
            ff.Invoke("11")
        End While
    End Sub

End Class
