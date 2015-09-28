Public Class DeskTask
    Public Sub New()
        Me.Date = Nothing
        Description = ""
        IsIndescriminate = False
    End Sub
    Public Sub New([Date] As Date, Description As String, IsIndescriminate As Boolean)
        Me.Date = [Date]
        Me.Description = Description
        Me.IsIndescriminate = IsIndescriminate
        If IsIndescriminate Then Me.Date = Date.Now
    End Sub
    Public Property IsIndescriminate As Boolean
    Public Property [Date] As Date
    Public Property Description As String
End Class
