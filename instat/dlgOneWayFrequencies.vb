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
Public Class dlgOneWayFrequencies
    Private bFirstLoad As Boolean = True
    Private bReset As Boolean = True
    Private bResetSubdialog As Boolean = False
    Private clsSjMiscFrq As New RFunction
    Private clsSjPlot As New RFunction
    Private clsPlotGrid As New RFunction
    Private clsSjPlotList As New RFunction
    Private clsAsGGplot As New RFunction
    Private clsAsDataFrame As New RFunction
    Private clsCaptureOutput As New RFunction
    Private clsLengthFunction As New RFunction
    Private clsLengthOperator As New ROperator
    Private clsPlusOperator As New ROperator
    Private clsDataFrame As New RFunction
    Private clsGgplotFuntion As New RFunction
    Private clsGeomTextFunction As New RFunction
    Private clsStemAndLeafFunction As New RFunction
    Private clsLabelAesFunction As New RFunction
    Private clsFlipCoordinates As New RFunction
    Private clsDummyFunction As New RFunction
    Public strDefaultDataFrame As String = ""
    Public strDefaultColumns() As String = Nothing

    Private Sub dlgOneWayFrequencies_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If bFirstLoad Then
            InitialiseDialog()
            bFirstLoad = False
        End If
        If bReset Then
            SetDefaults()
        End If
        SetRCodeForControls(bReset)
        SetDefaultColumn()
        bReset = False
        TestOkEnabled()
        autoTranslate(Me)
    End Sub

    Private Sub InitialiseDialog()
        ucrBase.iHelpTopicID = 518
        ucrBase.clsRsyntax.bExcludeAssignedFunctionOutput = False

        ucrReceiverOneWayFreq.SetParameter(New RParameter("x", 0))
        ucrReceiverOneWayFreq.SetParameterIsRFunction()
        ucrReceiverOneWayFreq.bForceAsDataFrame = True
        ucrReceiverOneWayFreq.Selector = ucrSelectorOneWayFreq
        ucrReceiverOneWayFreq.strSelectorHeading = "Variables"
        ucrReceiverOneWayFreq.bDropUnusedFilterLevels = True
        'temp fix to bug in sjPlot
        ucrReceiverOneWayFreq.bRemoveLabels = True

        ucrReceiverMultipleStemAndLeaf.SetParameter(New RParameter("x", 0))
        ucrReceiverMultipleStemAndLeaf.SetParameterIsRFunction()
        ucrReceiverMultipleStemAndLeaf.bWithQuotes = False
        ucrReceiverMultipleStemAndLeaf.Selector = ucrSelectorOneWayFreq
        ucrReceiverMultipleStemAndLeaf.SetIncludedDataTypes({"numeric"})

        ucrReceiverWeights.SetParameter(New RParameter("weight.by", 1))
        ucrReceiverWeights.SetParameterIsRFunction()
        ucrReceiverWeights.Selector = ucrSelectorOneWayFreq
        ucrReceiverWeights.SetDataType("numeric")
        ucrReceiverWeights.strSelectorHeading = "Numerics"

        ucrPnlSort.SetParameter(New RParameter("sort.frq", 2))
        ucrPnlSort.AddRadioButton(rdoNone, Chr(34) & "none" & Chr(34))
        ucrPnlSort.AddRadioButton(rdoAscending, Chr(34) & "asc" & Chr(34))
        ucrPnlSort.AddRadioButton(rdoDescending, Chr(34) & "desc" & Chr(34))
        ucrPnlSort.SetRDefault(Chr(34) & "none" & Chr(34))

        ucrChkWeights.SetText("Weights")
        ucrChkWeights.SetParameter(ucrReceiverWeights.GetParameter(), bNewChangeParameterValue:=False, bNewAddRemoveParameter:=True)
        ucrChkWeights.AddToLinkedControls(ucrReceiverWeights, {True}, bNewLinkedAddRemoveParameter:=True, bNewLinkedHideIfParameterMissing:=True)

        ucrPnlFrequencies.AddRadioButton(rdoTable)
        ucrPnlFrequencies.AddRadioButton(rdoGraph)
        ucrPnlFrequencies.AddRadioButton(rdoBoth)
        ucrPnlFrequencies.AddRadioButton(rdoStemAndLeaf)

        'setting rdoGraph, rdoTable and rdoStemAndLeaf
        ucrPnlFrequencies.AddParameterValuesCondition(rdoTable, "check", "table")
        ucrPnlFrequencies.AddParameterValuesCondition(rdoGraph, "check", "graph")
        ucrPnlFrequencies.AddParameterValuesCondition(rdoStemAndLeaf, "check", "stem")
        ucrPnlFrequencies.AddParameterValuesCondition(rdoBoth, "check", "both")
        'TODO. the both options can be added here when we are able to have panels conditions across multiple functions

        ucrPnlOutput.SetParameter(New RParameter("out", 7))
        ucrPnlOutput.AddRadioButton(rdoAsText, Chr(34) & "txt" & Chr(34))
        ucrPnlOutput.AddRadioButton(rdoAsHtml, Chr(34) & "viewer" & Chr(34))

        ucrPnlFrequencies.AddToLinkedControls(ucrChkFlip, {rdoGraph, rdoBoth}, bNewLinkedHideIfParameterMissing:=True, bNewLinkedAddRemoveParameter:=True)
        ucrPnlFrequencies.AddToLinkedControls(ucrSaveGraph, {rdoGraph, rdoBoth, rdoStemAndLeaf}, bNewLinkedHideIfParameterMissing:=True, bNewLinkedAddRemoveParameter:=True)
        ucrPnlFrequencies.AddToLinkedControls({ucrPnlSort, ucrChkWeights, ucrChkGroupData, ucrReceiverOneWayFreq}, {rdoTable, rdoGraph, rdoBoth}, bNewLinkedHideIfParameterMissing:=True, bNewLinkedAddRemoveParameter:=True)
        ucrPnlFrequencies.AddToLinkedControls({ucrReceiverMultipleStemAndLeaf, ucrChkScale}, {rdoStemAndLeaf}, bNewLinkedHideIfParameterMissing:=True, bNewLinkedAddRemoveParameter:=True)
        ucrPnlFrequencies.AddToLinkedControls(ucrPnlOutput, {rdoBoth, rdoTable}, bNewLinkedHideIfParameterMissing:=True)
        ucrPnlOutput.SetLinkedDisplayControl(grpOutput)

        ucrPnlFrequencies.AddToLinkedControls(ucrSaveDataFrame, {rdoTable, rdoBoth}, bNewLinkedHideIfParameterMissing:=True, bNewLinkedAddRemoveParameter:=True)
        ucrSaveDataFrame.SetPrefix("one_way_freq_table")
        ucrSaveDataFrame.SetSaveTypeAsDataFrame()
        ucrSaveDataFrame.SetDataFrameSelector(ucrSelectorOneWayFreq.ucrAvailableDataFrames)
        ucrSaveDataFrame.SetCheckBoxText("Save Table")
        ucrSaveDataFrame.SetIsComboBox()
        ucrSaveDataFrame.SetAssignToIfUncheckedValue("last_table")

        ucrNudGroups.SetParameter(New RParameter("auto.grp", 9))
        ucrNudGroups.SetMinMax(2, 100)
        ucrNudGroups.Increment = 5

        ucrChkGroupData.SetParameter(ucrNudGroups.GetParameter(), bNewChangeParameterValue:=False, bNewAddRemoveParameter:=True)
        ucrChkGroupData.SetText("Group Data")
        ucrChkGroupData.AddToLinkedControls(ucrNudGroups, {True}, bNewLinkedAddRemoveParameter:=True, bNewLinkedHideIfParameterMissing:=True, bNewLinkedChangeToDefaultState:=True, objNewDefaultState:=10)
        ucrChkGroupData.AddParameterPresentCondition(True, "auto.group")
        ucrChkGroupData.AddParameterPresentCondition(False, "auto.group", False)

        ucrChkMinFrq.SetText("Min Frequency")
        ucrChkMinFrq.AddToLinkedControls(ucrNudMinFreq, {True}, bNewLinkedAddRemoveParameter:=True, bNewLinkedHideIfParameterMissing:=True)
        ucrChkMinFrq.AddParameterPresentCondition(True, "min.frq", True)
        ucrChkMinFrq.AddParameterPresentCondition(False, "min.frq", False)

        ucrPnlFrequencies.AddToLinkedControls(ucrChkMinFrq, {rdoTable, rdoBoth}, bNewLinkedHideIfParameterMissing:=True)

        ucrNudMinFreq.SetParameter(New RParameter("min.frq", 10))
        ucrNudMinFreq.SetMinMax(iNewMin:=0)
        ucrNudMinFreq.SetRDefault(0)


        ucrChkFlip.SetParameter(New RParameter("coord.flip", 10))
        ucrChkFlip.SetText("Flip Coordinates")
        ucrChkFlip.SetValuesCheckedAndUnchecked("TRUE", "FALSE")
        ucrChkFlip.SetRDefault("FALSE")

        ucrChkScale.SetText("Scale")
        ucrChkScale.AddToLinkedControls(ucrNudScale, {True}, bNewLinkedAddRemoveParameter:=True, bNewLinkedHideIfParameterMissing:=True)
        ucrChkScale.AddParameterPresentCondition(True, "stem")
        ucrChkScale.AddParameterPresentCondition(False, "stem", False)

        ucrNudScale.SetParameter(New RParameter("scale", 1))
        ucrNudScale.AddParameterPresentCondition(True, "scale")
        ucrNudScale.SetMinMax(0.1)
        ucrNudScale.DecimalPlaces = 1
        ucrNudScale.Increment = 0.1

        ucrSaveGraph.SetPrefix("one_way_freq")
        ucrSaveGraph.SetSaveTypeAsGraph()
        ucrSaveGraph.SetDataFrameSelector(ucrSelectorOneWayFreq.ucrAvailableDataFrames)
        ucrSaveGraph.SetCheckBoxText("Save Graph")
        ucrSaveGraph.SetIsComboBox()
        ucrSaveGraph.SetAssignToIfUncheckedValue("last_graph")
    End Sub

    Private Sub SetDefaults()
        clsSjMiscFrq = New RFunction
        clsSjPlot = New RFunction
        clsPlotGrid = New RFunction
        clsAsGGplot = New RFunction
        clsAsDataFrame = New RFunction
        clsStemAndLeafFunction = New RFunction
        clsCaptureOutput = New RFunction
        clsLengthFunction = New RFunction
        clsLengthOperator = New ROperator
        clsPlusOperator = New ROperator
        clsDataFrame = New RFunction
        clsGgplotFuntion = New RFunction
        clsGeomTextFunction = New RFunction
        clsLabelAesFunction = New RFunction
        clsFlipCoordinates = New RFunction
        clsDummyFunction = New RFunction

        ucrSelectorOneWayFreq.Reset()
        ucrReceiverOneWayFreq.SetMeAsReceiver()
        ucrSaveGraph.Reset()
        ucrSaveDataFrame.Reset()

        clsDummyFunction.AddParameter("check", "table", iPosition:=0)

        clsAsDataFrame.SetRCommand("as.data.frame")
        clsAsDataFrame.AddParameter("x", clsRFunctionParameter:=clsSjMiscFrq, iPosition:=0)

        clsPlotGrid.SetPackageName("sjPlot")
        clsPlotGrid.SetRCommand("plot_grid")
        clsPlotGrid.AddParameter("x", clsRFunctionParameter:=clsSjPlotList, iPosition:=0)

        clsSjPlotList.SetRCommand("list")
        clsSjPlotList.AddParameter("x", clsRFunctionParameter:=clsSjPlot, iPosition:=0)

        clsSjMiscFrq.SetPackageName("sjmisc")
        clsSjMiscFrq.SetRCommand("frq")
        clsSjMiscFrq.AddParameter("out", Chr(34) & "txt" & Chr(34), iPosition:=7)

        clsSjPlot.SetPackageName("sjPlot")
        clsSjPlot.SetRCommand("plot_frq")
        clsSjPlot.AddParameter("geom.size", 0.5, iPosition:=14)
        clsSjPlot.SetAssignTo("one_way_plot")
        clsPlotGrid.SetAssignTo("last_graph", strTempDataframe:=ucrSelectorOneWayFreq.ucrAvailableDataFrames.cboAvailableDataFrames.Text, strTempGraph:="last_graph")

        clsAsGGplot.SetPackageName("ggplotify")
        clsAsGGplot.SetRCommand("as.ggplot")
        clsAsGGplot.AddParameter("plot", clsRFunctionParameter:=clsPlotGrid, iPosition:=0)

        clsStemAndLeafFunction.SetRCommand("stem")


        clsCaptureOutput.SetRCommand("capture.output")
        clsCaptureOutput.AddParameter("stem", clsRFunctionParameter:=clsStemAndLeafFunction, iPosition:=0, bIncludeArgumentName:=False)
        clsCaptureOutput.SetAssignTo("tmp")

        clsLengthFunction.SetRCommand("length")
        clsLengthFunction.AddParameter("tmp", clsRFunctionParameter:=clsCaptureOutput, iPosition:=0, bIncludeArgumentName:=False)

        clsLengthOperator.SetOperation(":")
        clsLengthOperator.AddParameter("start", 1, iPosition:=0)
        clsLengthOperator.AddParameter("end", clsRFunctionParameter:=clsLengthFunction, iPosition:=1)
        clsLengthOperator.SetAssignTo("rr")

        clsDataFrame.SetRCommand("data.frame")
        clsDataFrame.AddParameter("x", clsRFunctionParameter:=clsCaptureOutput, iPosition:=0, bIncludeArgumentName:=False)
        clsDataFrame.AddParameter("rr", clsROperatorParameter:=clsLengthOperator, iPosition:=1)
        clsDataFrame.SetAssignTo("stemdf")

        clsGgplotFuntion.SetPackageName("ggplot2")
        clsGgplotFuntion.SetRCommand("ggplot")
        clsGgplotFuntion.AddParameter("x", clsRFunctionParameter:=clsDataFrame, iPosition:=0)

        clsGeomTextFunction.SetPackageName("ggplot2")
        clsGeomTextFunction.SetRCommand("geom_text")
        clsGeomTextFunction.AddParameter("mapping", clsRFunctionParameter:=clsLabelAesFunction, iPosition:=0)
        clsGeomTextFunction.AddParameter("hjust", 0)

        clsLabelAesFunction.AddParameter("x", clsROperatorParameter:=clsLengthOperator, iPosition:=0)
        clsLabelAesFunction.AddParameter("y", 0, iPosition:=1)
        clsLabelAesFunction.AddParameter("label", clsRFunctionParameter:=clsCaptureOutput, iPosition:=2)

        clsLabelAesFunction.SetPackageName("ggplot2")
        clsLabelAesFunction.SetRCommand("aes")

        clsPlusOperator.SetOperation("+")
        clsPlusOperator.AddParameter("ggplot", clsRFunctionParameter:=clsGgplotFuntion, iPosition:=0)
        clsPlusOperator.AddParameter("geomtext", clsRFunctionParameter:=clsGeomTextFunction, iPosition:=1)
        clsPlusOperator.AddParameter("coord_flip", clsRFunctionParameter:=clsFlipCoordinates, iPosition:=2)

        clsFlipCoordinates.SetRCommand("coord_flip")

        ucrBase.clsRsyntax.SetBaseRFunction(clsSjMiscFrq)
        bResetSubdialog = True
    End Sub

    Public Sub SetRCodeForControls(bReset As Boolean)
        ucrChkWeights.AddAdditionalCodeParameterPair(clsSjPlot, ucrReceiverWeights.GetParameter(), iAdditionalPairNo:=1)
        ucrReceiverWeights.AddAdditionalCodeParameterPair(clsSjPlot, ucrReceiverWeights.GetParameter(), iAdditionalPairNo:=1)
        ucrPnlSort.AddAdditionalCodeParameterPair(clsSjPlot, New RParameter("sort.frq", 3), iAdditionalPairNo:=1)
        ucrNudGroups.AddAdditionalCodeParameterPair(clsSjPlot, New RParameter("auto.group", 9), iAdditionalPairNo:=1)
        ucrChkGroupData.AddAdditionalCodeParameterPair(clsSjPlot, New RParameter("auto.group", 9), iAdditionalPairNo:=1)
        ucrReceiverOneWayFreq.AddAdditionalCodeParameterPair(clsSjPlot, New RParameter("data", 0), iAdditionalPairNo:=1)
        ucrNudScale.AddAdditionalCodeParameterPair(clsStemAndLeafFunction, New RParameter("scale", 1), iAdditionalPairNo:=1)
        ucrSaveGraph.AddAdditionalRCode(clsPlusOperator, iAdditionalPairNo:=1)


        ucrReceiverWeights.SetRCode(clsSjMiscFrq, bReset)
        ucrReceiverOneWayFreq.SetRCode(clsSjMiscFrq, bReset)
        ucrPnlFrequencies.SetRCode(clsDummyFunction, bReset)


        ucrPnlOutput.SetRCode(clsSjMiscFrq, bReset)
        ucrChkWeights.SetRCode(clsSjMiscFrq, bReset)
        ucrPnlSort.SetRCode(clsSjMiscFrq, bReset)
        ucrChkFlip.SetRCode(clsSjPlot, bReset)
        ucrChkGroupData.SetRCode(clsSjMiscFrq, bReset)
        ucrNudGroups.SetRCode(clsSjMiscFrq, bReset)
        ucrSaveGraph.SetRCode(clsAsGGplot, bReset)
        ucrNudMinFreq.SetRCode(clsSjMiscFrq, bReset)
        ucrChkMinFrq.SetRCode(clsSjMiscFrq, bReset)
        ucrReceiverMultipleStemAndLeaf.SetRCode(clsStemAndLeafFunction, bReset)

        ucrSaveDataFrame.SetRCode(clsAsDataFrame, bReset)

    End Sub

    Private Sub SetDefaultColumn()
        If strDefaultDataFrame <> "" Then
            ucrSelectorOneWayFreq.SetDataframe(strDefaultDataFrame)
        End If
        If strDefaultColumns IsNot Nothing AndAlso strDefaultColumns.Count > 0 Then
            For Each strVar As String In strDefaultColumns
                ucrReceiverOneWayFreq.Add(strVar, strDefaultDataFrame)
            Next
        End If
        strDefaultDataFrame = ""
        strDefaultColumns = Nothing
    End Sub

    Private Sub TestOkEnabled()
        If rdoStemAndLeaf.Checked Then
            If Not ucrReceiverMultipleStemAndLeaf.IsEmpty Then
                ucrBase.OKEnabled(True)
            Else
                ucrBase.OKEnabled(False)
            End If
        Else
            If Not ucrReceiverOneWayFreq.IsEmpty() AndAlso ((ucrChkGroupData.Checked AndAlso ucrNudGroups.GetText <> "") OrElse Not ucrChkGroupData.Checked) AndAlso ucrSaveGraph.IsComplete() AndAlso ucrSaveDataFrame.IsComplete() Then
                If ucrChkWeights.Checked Then
                    If Not ucrReceiverWeights.IsEmpty Then
                        ucrBase.OKEnabled(True)
                    Else
                        ucrBase.OKEnabled(False)
                    End If
                Else
                    ucrBase.OKEnabled(True)
                End If
            Else
                ucrBase.OKEnabled(False)
            End If
        End If

    End Sub

    Private Sub ucrBase_ClickReset(sender As Object, e As EventArgs) Handles ucrBase.ClickReset
        SetDefaults()
        SetRCodeForControls(True)
        TestOkEnabled()
    End Sub

    Private Sub ucrChkWeights_ControlValueChanged(ucrChangedControl As ucrCore) Handles ucrChkWeights.ControlValueChanged
        If ucrChkWeights.Checked Then
            ucrReceiverWeights.SetMeAsReceiver()
        Else
            ucrReceiverOneWayFreq.SetMeAsReceiver()
        End If
    End Sub

    Private Sub cmdOptions_Click(sender As Object, e As EventArgs) Handles cmdOptions.Click
        sdgOneWayFrequencies.SetRFunction(clsSjMiscFrq, clsSjPlot, clsPlotGrid, clsSjPlotList, bResetSubdialog)
        bResetSubdialog = False
        sdgOneWayFrequencies.ShowDialog()
        TestOkEnabled()
    End Sub

    Private Sub ucrPnlFrequencies_ControlValueChanged(ucrChangedControl As ucrCore) Handles ucrPnlFrequencies.ControlValueChanged, ucrSaveDataFrame.ControlValueChanged
        SetBaseFunction()
    End Sub

    Private Sub SetBaseFunction()
        ucrBase.clsRsyntax.GetBeforeCodes().Clear()
        ucrReceiverOneWayFreq.SetMeAsReceiver()
        grpSort.Visible = True
        cmdOptions.Visible = True
        If rdoGraph.Checked Then
            ucrBase.clsRsyntax.SetBaseRFunction(clsAsGGplot)
            ucrBase.clsRsyntax.iCallType = 3
            clsDummyFunction.AddParameter("check", "graph", iPosition:=0)
        Else
            If rdoBoth.Checked Then
                ucrBase.clsRsyntax.AddToBeforeCodes(If(ucrSaveDataFrame.ucrChkSave.Checked AndAlso ucrSaveDataFrame.IsComplete, clsAsDataFrame, clsSjMiscFrq))
                ucrBase.clsRsyntax.SetBaseRFunction(clsAsGGplot)
                ucrBase.clsRsyntax.iCallType = 3
                clsDummyFunction.AddParameter("check", "both", iPosition:=0)
            ElseIf rdoTable.Checked Then
                ucrBase.clsRsyntax.SetBaseRFunction(If(ucrSaveDataFrame.ucrChkSave.Checked AndAlso ucrSaveDataFrame.IsComplete, clsAsDataFrame, clsSjMiscFrq))
                ucrBase.clsRsyntax.iCallType = 2
                clsDummyFunction.AddParameter("check", "table", iPosition:=0)
            Else
                ucrReceiverMultipleStemAndLeaf.SetMeAsReceiver()
                grpSort.Visible = False
                cmdOptions.Visible = False
                ucrBase.clsRsyntax.SetBaseROperator(clsPlusOperator)
                ucrBase.clsRsyntax.iCallType = 3
                clsDummyFunction.AddParameter("check", "stem", iPosition:=0)
            End If
        End If

    End Sub

    Private Sub ucrReceiverOneWayFreq_ControlValueChanged(ucrChangedControl As ucrCore) Handles ucrReceiverOneWayFreq.ControlValueChanged
        clsPlotGrid.AddParameter("tags", ucrReceiverOneWayFreq.GetVariableNames, iPosition:=1)
        If ucrReceiverOneWayFreq.lstSelectedVariables.Items.Count > 1 Then
            clsPlotGrid.AddParameter("x", clsRFunctionParameter:=clsSjPlot, iPosition:=0)
        Else
            clsPlotGrid.AddParameter("x", clsRFunctionParameter:=clsSjPlotList, iPosition:=0)
        End If
    End Sub

    Private Sub Controls_ControlContentsChanged(ucrChangedControl As ucrCore) Handles ucrReceiverWeights.ControlContentsChanged, ucrChkWeights.ControlContentsChanged,
        ucrNudGroups.ControlContentsChanged, ucrChkGroupData.ControlContentsChanged, ucrReceiverOneWayFreq.ControlContentsChanged, ucrSaveGraph.ControlContentsChanged,
        ucrNudMinFreq.ControlValueChanged, ucrChkMinFrq.ControlValueChanged, ucrSaveDataFrame.ControlValueChanged, ucrReceiverMultipleStemAndLeaf.ControlContentsChanged
        TestOkEnabled()
    End Sub
End Class
