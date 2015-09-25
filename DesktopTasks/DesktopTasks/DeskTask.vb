Public Class DeskTask
    Public Sub New()
        Me.Date = Nothing
        Description = ""
    End Sub
    Public Sub New([Date] As Date, Description As String)
        Me.Date = [Date]
        Me.Description = Description
    End Sub
    Public Property [Date] As Date
    Public Property Description As String
End Class
