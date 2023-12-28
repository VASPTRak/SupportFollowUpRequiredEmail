Imports log4net
Imports log4net.Config
Imports System.Data.SqlClient

Public Class MasterBAL
    Private Shared ReadOnly log As ILog = LogManager.GetLogger(GetType(MasterBAL))

    Shared Sub New()
        XmlConfigurator.Configure()
    End Sub

    Public Function GetSupportFollowUpRequiredData() As DataTable
        Dim dal = New GeneralizedDAL()
        Try

            Dim ds As DataSet = New DataSet()

            Dim Param As SqlParameter() = New SqlParameter() {}

            ds = dal.ExecuteStoredProcedureGetDataSet("usp_tt_Support_GetGetSupportFollowUpRequiredData", Param)

            Return ds.Tables(0)

        Catch ex As Exception

            log.Error("Error occurred in GetSupportFollowUpRequiredData Exception is :" + ex.Message)
            Return Nothing
        Finally

        End Try
    End Function

    Public Function UpdateSupportFollowUpRequiredFlag(SupportId As Integer) As Integer
        Dim dal = New GeneralizedDAL()
        Try

            Dim result As Integer = 0

            Dim Param As SqlParameter() = New SqlParameter(0) {}

            Param(0) = New SqlParameter("@SupportItemId", SqlDbType.Int)
            Param(0).Direction = ParameterDirection.Input
            Param(0).Value = SupportId

            result = dal.ExecuteStoredProcedureParaGetInteger("usp_tt_Support_UpdateSupportFollowUpRequiredFlag", Param)

            Return result

        Catch ex As Exception

            log.Error("Error occurred in UpdateSupportFollowUpRequiredFlag Exception is :" + ex.Message)
            Return 0
        Finally

        End Try
    End Function
End Class