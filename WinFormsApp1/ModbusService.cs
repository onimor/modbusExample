using NModbus;
using System.Net;
using System.Net.Sockets;

namespace WinFormsApp1;
public class ModbusService {
    
    private readonly byte _slaveID = 1;
    private readonly int _port = 502;
    private readonly string _ip = "127.0.0.1";
    private CancellationTokenSource _cancelTokenSource;
    private TcpClient _client;
    private ModbusFactory _factory;
    private IModbusMaster _master; 

    //Для запуска своего tcp slave
    public async void StartModbusTcpSlave() {
        _cancelTokenSource = new();
        await Task.Run(() => {
            var address = IPAddress.Parse(_ip);

            // создаем и запускаем tcp подключение
            var slaveTcpListener = new TcpListener(address, _port);
            slaveTcpListener.Start();

            _factory = new ModbusFactory(); 
            //создаем slave и передаем туда наше подключение
            var network = _factory.CreateSlaveNetwork(slaveTcpListener);
            //создаем хранилище для slave, где будут наши данные, передаем id так как таких хранилищ может быть много
            var slave1 = _factory.CreateSlave(_slaveID);
            //добавляем его в наш slave 
            network.AddSlave(slave1); 
            network.ListenAsync(_cancelTokenSource.Token);
        }, _cancelTokenSource.Token); 
    }
    public void StopTCPSlave() {
        _cancelTokenSource.Cancel();
        _cancelTokenSource.Dispose(); 
    }

    //инициализируем master
    public async Task InitMaster() {
        if (_client is not null) {
            _client.Close();
            _client.Dispose();
        } 
        //создаем tcp клиента для конекта
        _client = new();
        await _client.ConnectAsync(_ip, _port);

        // инициализируем modbus master
        _master = _factory.CreateMaster(_client);
    }

    //читаем из регистра
    public async Task<ushort[]> ReadRegisters() {
        ushort startAddress = 1;//с какого регистра начинать чтение
        ushort readRegisterCount = 10;//сколько регистров читать (максимум 124 можно за раз, я думаю понятно почему) 
        
        // читаем регистры и возвращаем результат
        return await _master.ReadHoldingRegistersAsync(_slaveID, startAddress, readRegisterCount);
    }
    public async Task SendDataToRegisters(IReadOnlyList<ushort> ushorts) {
        ushort startAddress = 1;
        await _master.WriteMultipleRegistersAsync(_slaveID, startAddress, ushorts.ToArray());
    }
}
