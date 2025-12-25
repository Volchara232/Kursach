using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Sem3_kurs.Model;
using Sem3_kurs.Collections;
namespace Sem3_kurs.ViewModels
{
    public class ClientViewModel : INotifyPropertyChanged
    {
        private Client _client;
        private EstateAgency _agency;

        public ClientViewModel(Client client, EstateAgency agency)
        {
            _client = client;
            _agency = agency;

            Orders = new ObservableCollection<Order>(client.Orders);
            ActiveOrders = new ObservableCollection<Order>(client.GetActiveOrders());
        }

   
        public string FullName
        {
            get => _client.FullName;
            set
            {
                if (_client.FullName != value)
                {
                    _client.FullName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Address
        {
            get => _client.Address;
            set
            {
                if (_client.Address != value)
                {
                    _client.Address = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Phone
        {
            get => _client.Phone;
            set
            {
                if (_client.Phone != value)
                {
                    _client.Phone = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Email
        {
            get => _client.Email;
            set
            {
                if (_client.Email != value)
                {
                    _client.Email = value;
                    OnPropertyChanged();
                }
            }
        }

        public string RegistrationNumber
        {
            get => _client.RegistrationNumber;
            set
            {
                if (_client.RegistrationNumber != value)
                {
                    _client.RegistrationNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Order> Orders { get; private set; }
        public ObservableCollection<Order> ActiveOrders { get; private set; }

  
        public int TotalOrdersCount => Orders.Count;
        public int ActiveOrdersCount => ActiveOrders.Count;


        public void RefreshOrders()
        {
            Orders.Clear();
            ActiveOrders.Clear();

            foreach (var order in _client.Orders)
            {
                Orders.Add(order);
            }

            foreach (var order in _client.GetActiveOrders())
            {
                ActiveOrders.Add(order);
            }

            OnPropertyChanged(nameof(TotalOrdersCount));
            OnPropertyChanged(nameof(ActiveOrdersCount));
        }


        public void AddNewOrder(Order order)
        {
            if (order == null) return;

            _client.AddOrder(order);
            _agency.OrdersList.Add(order);
            RefreshOrders();
        }

        public Client GetModel() => _client;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}