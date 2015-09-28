Public Class DeskTask
    Public Sub New()
        Me._Date = Nothing
        Description = ""
        IsIndescriminate = False
    End Sub
    Public Sub New([Date] As Date, Description As String, IsIndescriminate As Boolean)
        Me._Date = [Date]
        Me.Description = Description
        Me.IsIndescriminate = IsIndescriminate
        If IsIndescriminate Then Me._Date = Date.Now
    End Sub
    Public Property IsIndescriminate As Boolean
    Private _Date As Date
    <System.ComponentModel.Browsable(False)>
    Public Property [Date] As Date
        Get
            Return _Date
        End Get
        Set(value As Date)
            _Date = value
        End Set
    End Property
    Public ReadOnly Property TaskDate As Date
        Get
            Return _Date
        End Get
    End Property
    Public Property Description As String
End Class
