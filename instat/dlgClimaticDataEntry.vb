﻿' R- Instat
' Copyright (C) 2015-2017
'
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License 
' along with this program.  If not, see <http://www.gnu.org/licenses/>.

Imports instat.Translations
Imports RDotNet
Imports unvell.ReoGrid
Public Class dlgClimaticDataEntry
    Public bFirstLoad As Boolean = True
    Private bReset As Boolean = True
    Private clsClimaticDataEntry As New RFunction
    Private clsDataChangedFunction As New RFunction

    'Public dfTemp As DataFrame
    Private Sub dlgClimaticDataEntry_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If bFirstLoad Then
            InitialiseDialog()
            bFirstLoad = False
        End If
        If bReset Then
            SetDefaults()
        End If
        SetRCodeForControls(bReset)
        bReset = False
        autoTranslate(Me)
    End Sub
    Private Sub InitialiseDialog()

        ucrBase.iHelpTopicID = 359

        'ucrSelectorClimaticDataEntry.SetParameter(New RParameter("data_name", 0))
        'ucrSelectorClimaticDataEntry.SetParameterIsString()

        'ucrReceiverStation.SetParameter(New RParameter("station", 1))
        ucrReceiverStation.Selector = ucrSelectorClimaticDataEntry
        ucrReceiverStation.SetClimaticType("station")
        ucrReceiverStation.SetParameterIsRFunction()
        ucrReceiverStation.bAutoFill = True
        ucrReceiverStation.strSelectorHeading = "Factors"

        'todo. how do we make this control write parameter value with quotes while displaying without quotes
        'if the items are being set by the receiver. New enhancement??
        'ucrInputSelectStation.SetParameter(New RParameter("station_name", 2))
        ucrInputSelectStation.SetFactorReceiver(ucrReceiverStation)
        ucrInputSelectStation.SetItems()
        ucrInputSelectStation.strQuotes = ""

        'ucrReceiverDate.SetParameter(New RParameter("date", 3))
        ucrReceiverDate.Selector = ucrSelectorClimaticDataEntry
        ucrReceiverDate.SetClimaticType("date")
        ucrReceiverDate.SetIncludedDataTypes({"Date"})
        ucrReceiverDate.bAutoFill = True
        ucrReceiverDate.SetParameterIsRFunction()
        ucrReceiverDate.strSelectorHeading = "Date"

        'ucrReceiverElements.SetParameter(New RParameter("elements", 4))
        ucrReceiverElements.Selector = ucrSelectorClimaticDataEntry
        ucrReceiverElements.SetParameterIsRFunction()
        ucrReceiverElements.strSelectorHeading = "Numerics"
        ucrReceiverElements.SetIncludedDataTypes({"numeric"})
        ucrReceiverElements.SetClimaticType("element")
        ucrReceiverElements.bAutoFill = True

        'ucrDateTimePickerStartingDate.SetParameter(New RParameter("start_date", 5))
        ucrDateTimePickerStartingDate.SetParameterIsRDate()

        ucrPnlOptions.AddRadioButton(rdoDaily)
        ucrPnlOptions.AddRadioButton(rdoMonthly)

    End Sub
    Private Sub SetDefaults()
        clsClimaticDataEntry = New RFunction
        clsDataChangedFunction = New RFunction

        ucrSelectorClimaticDataEntry.Reset()
        ucrReceiverElements.SetMeAsReceiver()

        clsDataChangedFunction.SetRCommand("data.frame")

        'e,g data_book$save_data_entry_data(data_name="Sheet1", new_data = data.frame(station = "A", date = as.Date(c("1999/1/1", "1999/1/2", "1999/1/3")), rain = c(1, 5, 3), tmax = c(43, 53, 2)), rows_changed = c(1, 4, 3))
        clsClimaticDataEntry.SetRCommand(frmMain.clsRLink.strInstatDataObject & "$save_data_entry_data")

        ucrBase.clsRsyntax.SetBaseRFunction(clsClimaticDataEntry)
    End Sub

    Private Sub SetRCodeForControls(bReset As Boolean)
        'ucrSelectorClimaticDataEntry.SetRCode(clsClimaticDataEntry, bReset)
        'ucrInputSelectStation.SetRCode(clsClimaticDataEntry, bReset)
        'ucrReceiverDate.SetRCode(clsClimaticDataEntry, bReset)
        'ucrReceiverElements.SetRCode(clsClimaticDataEntry, bReset)
        'ucrDateTimePickerStartingDate.SetRCode(clsClimaticDataEntry, bReset)
    End Sub

    Private Sub TestOkEnabled()
        If Not ucrReceiverDate.IsEmpty AndAlso Not ucrReceiverElements.IsEmpty Then
            ucrBase.OKEnabled(sdgClimaticDataEntry.GetNumofRowsChanged > 0)
            cmdEnterData.Enabled = True
        Else
            ucrBase.OKEnabled(False)
            cmdEnterData.Enabled = False
        End If
    End Sub
    Private Sub ucrBase_ClickReset(sender As Object, e As EventArgs) Handles ucrBase.ClickReset
        SetDefaults()
        sdgClimaticDataEntry.Reset()
        SetRCodeForControls(True)
        TestOkEnabled()
    End Sub
    Private Sub ucrControls_ControlContentsChanged(ucrChangedControl As ucrCore) Handles ucrReceiverStation.ControlContentsChanged, ucrInputSelectStation.ControlContentsChanged, ucrReceiverDate.ControlContentsChanged, ucrReceiverElements.ControlContentsChanged
        TestOkEnabled()
    End Sub

    Private Sub cmdEnterData_Click(sender As Object, e As EventArgs) Handles cmdEnterData.Click
        Dim strStationColumnName As String = ucrReceiverStation.GetVariableNames(bWithQuotes:=False)
        Dim strDateColumnName As String = ucrReceiverDate.GetVariableNames(bWithQuotes:=False)
        Dim lstElementsColumnNames As List(Of String) = ucrReceiverElements.GetVariableNamesList(bWithQuotes:=False).ToList
        Dim strStationSelected As String = ucrInputSelectStation.GetValue()
        Dim startDateSelected As Date = ucrDateTimePickerStartingDate.DateValue
        Dim strSelectedDataFrameName As String = ucrSelectorClimaticDataEntry.strCurrentDataFrame
        Dim dfTemp As DataFrame = GetSelectedDataFrame()

        If dfTemp Is Nothing Then
            'todo. display message box here. This could be due to no records from the starting date
            Return
        End If

        'todo. should we have the abilty to retain the previous changes if the selections have not changed??
        sdgClimaticDataEntry.Setup(strStationColumnName, strDateColumnName, lstElementsColumnNames,
                                   dfTemp, strSelectedDataFrameName)

        sdgClimaticDataEntry.ShowDialog()

        'if no changes made then just return
        If sdgClimaticDataEntry.GetNumofRowsChanged < 1 Then
            Return
        End If

        'add the station column if it was selected. e.g Added as station = "sample_name" 
        If Not String.IsNullOrEmpty(strStationColumnName) AndAlso Not String.IsNullOrEmpty(strStationSelected) Then
            clsDataChangedFunction.AddParameter(strStationColumnName, Chr(34) & strStationSelected & Chr(34), iPosition:=0)
        End If

        'add the date column with its values as date e.g date = as.Date(c("1999/1/1", "1999/1/2"))
        clsDataChangedFunction.AddParameter(strDateColumnName,
                                "as.Date(" & sdgClimaticDataEntry.GetRowsChangedAsRVectorString(strDateColumnName, Chr(34)) & ")", iPosition:=1)

        'add the elements individual columns with their value e.g rain = c(1, 5, 3)
        For Each strElementName As String In lstElementsColumnNames
            clsDataChangedFunction.AddParameter(strElementName, sdgClimaticDataEntry.GetRowsChangedAsRVectorString(strElementName))
        Next

        'set the parameters for data changed and rows changed of the data frame that was passed to the dialog
        clsClimaticDataEntry.AddParameter("data_name", Chr(34) & strSelectedDataFrameName & Chr(34), iPosition:=0)
        clsClimaticDataEntry.AddParameter("new_data", clsRFunctionParameter:=clsDataChangedFunction, iPosition:=1)
        clsClimaticDataEntry.AddParameter("rows_changed", sdgClimaticDataEntry.GetRowNamesChangedAsRVectorString, iPosition:=2)
      
        TestOkEnabled()
    End Sub

    Private Function GetSelectedDataFrame() As DataFrame
        Dim dfTemp As DataFrame = Nothing
        Dim clsGetDataFrame As New RFunction
        Dim expTemp As SymbolicExpression

        clsGetDataFrame.SetRCommand(frmMain.clsRLink.strInstatDataObject & "$get_data_entry_data")
        clsGetDataFrame.AddParameter("data_name", Chr(34) & ucrSelectorClimaticDataEntry.strCurrentDataFrame & Chr(34), iPosition:=0)
        clsGetDataFrame.AddParameter("station", ucrReceiverStation.GetVariableNames, iPosition:=1)
        clsGetDataFrame.AddParameter("date", ucrReceiverDate.GetVariableNames, iPosition:=2)
        clsGetDataFrame.AddParameter("elements", ucrReceiverElements.GetVariableNames(), iPosition:=3)
        clsGetDataFrame.AddParameter("station_name", Chr(34) & ucrInputSelectStation.GetValue() & Chr(34), iPosition:=4)
        clsGetDataFrame.AddParameter("start_date", clsRFunctionParameter:=ucrDateTimePickerStartingDate.ValueAsRDate(), iPosition:=5)
        expTemp = frmMain.clsRLink.RunInternalScriptGetValue(clsGetDataFrame.ToScript(), bSilent:=True)
        If expTemp IsNot Nothing Then
            dfTemp = expTemp.AsDataFrame()
        End If
        Return dfTemp
    End Function


End Class