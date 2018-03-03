-- 1 = RAM Watch Overlay
-- 2 = Address Ripper to File
-- 3 = Dump all "debug function" locations
scriptFlag = 2

function getLoadAddresses(searchSpace, upperBound, lowerBound)
  addresses = {}
  local level_counter = 0
  for i=0, searchSpace.Count-1 do
    addr = tonumber(searchSpace.Address[i], 16)
    if addr > lowerBound and addr < upperBound then
      addresses[level_counter] = addr
      level_counter = level_counter + 1
    end
    if addr > upperBound then
      return addresses
    end
  end
  return addresses
end

if scriptFlag == 1 then
  RWCEMainDirectory = [[E:\Emulator\ramwatches]]
  RWCEOptions = {
    gameModuleName = 'jak2',
    gameVersion = 'na_0_00',
    layoutName = 'Jak2Layout',
    windowPosition = {0, 0},
  }
  local loaderFile, errorMessage = loadfile(RWCEMainDirectory .. '/loader.lua')
  if errorMessage then error(errorMessage) end
  loaderFile()
elseif scriptFlag == 2 then
  local START_ADDRESS = 0x20600000 -- 20200000
  local END_ADDRESS = 0x20900000

  local headerScan = createMemScan()
  headerScan.firstScan(soExactValue, vtByteArray, rtRounded, "54 34 62 00", "", START_ADDRESS, END_ADDRESS, "",
                fsmNotAligned, "1", true, false, false, false)
  headerScan.waitTillDone()
  local headers = createFoundList(headerScan)
  headers.initialize();
  local loadPtrScan = createMemScan()
  loadPtrScan.firstScan(soExactValue, vtByteArray, rtRounded, "01 25 14 00", "", START_ADDRESS, END_ADDRESS, "",
                fsmNotAligned, "1", true, false, false, false)
  loadPtrScan.waitTillDone()
  local loadPtrs = createFoundList(loadPtrScan)
  loadPtrs.initialize();

  for h=0, headers.Count-1 do
    local str_len_lea = tonumber(headers.Address[h], 16)+4
    print("------------")
    print("Found Struct")
    print("Effective Address - " .. string.format("%x", str_len_lea))

    local bytes = readBytes(str_len_lea, 3, true)
    local str_len_ptr = ""
    for k, val in pairs(bytes) do
        str_len_ptr = string.format("%x", val) .. str_len_ptr
    end
    str_len_ptr = "20" .. str_len_ptr

    str_len_ptr = tonumber(str_len_ptr, 16)
    local str_len = readInteger(str_len_ptr)
    local struct_name = readString(str_len_ptr+4, str_len)
    print("Name - " .. struct_name)

    loadAddresses = getLoadAddresses(loadPtrs, str_len_ptr, tonumber(headers.Address[h], 16))
    for k, val in pairs(loadAddresses) do
      local load_val = readBytes(val-4, 4, true)
      load_val_hex = ""
      for i=1, 4 do
        load_val_hex = load_val_hex .. " " .. string.format("%02x", load_val[i])
      end
      print("Level " .. k+1 .. " - " .. load_val_hex)
    end
    collectgarbage("collect")
  end
  headers.destroy()
  headerScan.destroy()
end
  
  
  
