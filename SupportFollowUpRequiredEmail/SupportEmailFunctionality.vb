Imports System.Configuration
Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports log4net
Imports log4net.Config

Module SupportEmailFunctionality

    Private ReadOnly log As ILog = LogManager.GetLogger(GetType(SupportEmailFunctionality))
    Sub Main()
        Try
            XmlConfigurator.Configure()
            log.Info("In Support Follow Up Required Email Functionality..")
            GetSupportFollowUpRequiredDataAndSendEmail()
        Catch ex As Exception
            log.Error("Error in Main - " & ex.ToString)
        End Try

    End Sub

    Private Sub GetSupportFollowUpRequiredDataAndSendEmail()
        Try
            log.Info("In GetSupportFollowUpRequiredDataAndSendEmail")
            Dim supportBal As MasterBAL = New MasterBAL()
            Dim dtData As DataTable = New DataTable()
            dtData = supportBal.GetSupportFollowUpRequiredData()

            If (dtData IsNot Nothing) Then
                If (dtData.Rows.Count > 0) Then
                    For index = 0 To dtData.Rows.Count - 1
                        Try
                            Dim emailSendTo As String = ConfigurationManager.AppSettings("EmailSendTO")
                            Dim OwnerEmail As String = ConfigurationManager.AppSettings("OwnerEmail")
                            Dim CompanyName As String = dtData.Rows(index)("CompanyName")
                            Dim IssueType As String = dtData.Rows(index)("IssueTypeText")
                            Try

                                Dim body As String = String.Empty
                                Using sr As New StreamReader(ConfigurationManager.AppSettings("PathForSupportFollowUpEmailTemplate"))
                                    body = sr.ReadToEnd()
                                End Using
                                '------------------

                                body = body.Replace("owneremail", OwnerEmail)

                                Try
                                    body = body.Replace("ImageSign", "<img src=""https://www.fluidsecure.net/Content/Images/FluidSECURELogo.png"" style=""width:200px""/>")
                                Catch ex As Exception
                                    body = body.Replace("ImageSign", "")
                                End Try

                                Dim mailClient As New SmtpClient(ConfigurationManager.AppSettings("smtpServer"))
                                mailClient.UseDefaultCredentials = False
                                mailClient.Credentials = New NetworkCredential(ConfigurationManager.AppSettings("emailAccount"), ConfigurationManager.AppSettings("emailPassword"))
                                mailClient.Port = Convert.ToInt32(ConfigurationManager.AppSettings("smtpPort"))

                                Dim messageSend As New MailMessage()
                                messageSend.Body = body
                                messageSend.IsBodyHtml = True
                                messageSend.Subject = "**Subject:  Follow up required for " & CompanyName & " regarding their " & IssueType & " issue."
                                messageSend.From = New MailAddress(ConfigurationManager.AppSettings("FromEmail"))
                                mailClient.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings("EnableSsl"))

                                If emailSendTo <> "" Then
                                    Try
                                        messageSend.To.Add(emailSendTo.Trim()) '
                                        mailClient.Send(messageSend)
                                        log.Info("Email send to: " & emailSendTo & " Support Id - " & dtData.Rows(index)("SupportItemId").ToString())
                                        messageSend.To.Remove(New MailAddress(emailSendTo.Trim()))
                                        'Uncheck flag of follow up
                                        supportBal = New MasterBAL()
                                        supportBal.UpdateSupportFollowUpRequiredFlag(Convert.ToInt32(dtData.Rows(index)("SupportItemId")))
                                    Catch ex As Exception
                                        log.Error("Error in GetSupportFollowUpRequiredData while updating follow up flag - " & ex.ToString)
                                    End Try
                                End If
                            Catch ex As Exception
                                log.Debug("Exception occurred in while sending unresolved email to " & OwnerEmail & " . ex is :" & ex.ToString())
                            End Try

                        Catch ex As Exception
                            log.Error("Error in GetSupportFollowUpRequiredData while fetching data - " & ex.ToString)
                        End Try
                    Next
                Else
                    log.Info("GetSupportFollowUpRequiredDataAndSendEmail - No Data Found")
                End If
            Else
                log.Info("GetSupportFollowUpRequiredDataAndSendEmail - No Data Found")
            End If
        Catch ex As Exception
            log.Error("Error in GetSupportFollowUpRequiredData - " & ex.ToString)
        End Try
    End Sub
End Module
