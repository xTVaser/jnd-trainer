-- firstScan(scanoption, vartype, roundingtype, input1,
  --   input2 ,startAddress ,stopAddress ,protectionflags ,
  --   alignmenttype ,"alignmentparam" ,isHexadecimalInput ,
  --   isNotABinaryString, isunicodescan, iscasesensitive);

  --[[
    scanOption: Defines what type of scan is done. Valid values for firstscan are:
    soUnknownValue: Unknown initial value scan
    soExactValue: Exact Value scan
    soValueBetween: Value between scan
    soBiggerThan: Bigger than ... scan
    soSmallerThan: smaller than ... scan
  ]]--

  --[[
    vartype: Defines the variable type. Valid variable types are:
      vtByte
      vtWord  2 bytes
      vtDword 4 bytes
      vtQword 8 bytes
      vtSingle float
      vtDouble
      vtString
      vtByteArray
      vtGrouped
      vtBinary
      vtAll
  ]]
