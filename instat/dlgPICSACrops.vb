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

Public Class dlgPICSACrops
    Private clsCropsFunction As New RFunction
    Public bFirstLoad As Boolean = True
    Private bReset As Boolean = True

    Private Sub dlgPICSACrops_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        autoTranslate(Me)
        If bFirstLoad Then
            InitialiseDialog()
            bFirstLoad = False
        End If
        If bReset Then
            SetDefaults()
        End If
        SetRCodeForControls(bReset)
        bReset = False

        TestOKEnabled()
    End Sub

    Private Sub InitialiseDialog()
        ucrBase.iHelpTopicID = 480


        'Selector
        ucrSelectorForCrops.SetParameter(New RParameter("data_name", 0))
        ucrSelectorForCrops.SetParameterIsString()

        'Station Receiver
        ucrReceiverStation.SetParameter(New RParameter(" ", 1))
        ucrReceiverStation.SetParameterIsString()
        ucrReceiverStation.Selector = ucrSelectorForCrops
        ucrReceiverStation.SetMeAsReceiver()

        'Date Receiver
        ucrReceiverDate.SetParameter(New RParameter(" ", 2))
        ucrReceiverDate.SetParameterIsString()
        ucrReceiverDate.Selector = ucrSelectorForCrops

        'Start Receiver
        ucrReceiverStart.SetParameter(New RParameter(" ", 3))
        ucrReceiverStart.SetParameterIsString()
        ucrReceiverStart.Selector = ucrSelectorForCrops

        'End Receiver
        ucrReceiverEnd.SetParameter(New RParameter(" ", 4))
        ucrReceiverEnd.SetParameterIsString()
        ucrReceiverEnd.Selector = ucrSelectorForCrops

        'Year Receiver
        ucrReceiverYear.SetParameter(New RParameter(" ", 4))
        ucrReceiverYear.SetParameterIsString()
        ucrReceiverYear.Selector = ucrSelectorForCrops

        'Day Receiver
        ucrReceiverDay.SetParameter(New RParameter(" ", 4))
        ucrReceiverDay.SetParameterIsString()
        ucrReceiverDay.Selector = ucrSelectorForCrops

        'Rainfall Receiver
        ucrReceiverRainfall.SetParameter(New RParameter(" ", 4))
        ucrReceiverRainfall.SetParameterIsString()
        ucrReceiverRainfall.Selector = ucrSelectorForCrops

        'Planting single
        ucrNudPlantingSingleDayNum.SetParameter(New RParameter(" ", 1))
        ucrNudPlantingSingleDayNum.SetMinMax(1, 366)

        'Planting Sequence From
        ucrNudPlantingSeqFrom.SetParameter(New RParameter("from", 1))
        ucrNudPlantingSeqFrom.SetMinMax(1, 366)

        'Planting Sequence Step
        ucrNudPlantingSeqStep.SetParameter(New RParameter("step", 1))
        ucrNudPlantingSeqStep.SetMinMax(1, 366)

        'Planting Sequence To
        ucrNudPlantingSeqTo.SetParameter(New RParameter("to", 1))
        ucrNudPlantingSeqTo.SetMinMax(1, 366)






        ' ucrSaveDataFrame
        ucrSaveDataFrame.SetIsTextBox()
        ucrSaveDataFrame.SetSaveTypeAsDataFrame()
        ucrSaveDataFrame.SetLabelText("Save result as:")
        ucrSaveDataFrame.SetPrefix("data")

    End Sub

    Private Sub SetDefaults()
        clsCropsFunction = New RFunction
        clsCropsFunction.SetRCommand(frmMain.clsRLink.strInstatDataObject & "$...")
        ucrBase.clsRsyntax.SetBaseRFunction(clsCropsFunction)
        TestOKEnabled()
    End Sub

    Public Sub SetRCodeForControls(bReset As Boolean)
        'SetRCode(Me, ucrBase.clsRsyntax.clsBaseFunction, bReset)
    End Sub

    Private Sub ucrBase_ClickReset(sender As Object, e As EventArgs) Handles ucrBase.ClickReset
        SetDefaults()
        SetRCodeForControls(True)
        TestOKEnabled()
    End Sub

    Private Sub TestOKEnabled()

    End Sub


End Class