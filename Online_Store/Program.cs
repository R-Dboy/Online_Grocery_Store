using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

//Apllication: Online Grocery Store
//Developer: Ralph Dagomboy

//Description: This application takes in several two parameter inputs, first parameter as quantity of items to be ordered
//             and the second parameter is the item number to be ordered. The application calculates the breakdown of 
//             prices per item based on the prices per pre-packed quantities.

//Assumption:
//          (1) Quantities less than the minimum packs are not invoiced.
//          (2) Input and other variables validation is not a concern at this point. 
//              However, it can easily be integrated to the application at a later stage.


namespace Online_Store
{
    class Program
    {
        //Main Program
        static void Main(string[] args)
        {
            //Creating Instance of Items and corresponding price per pack 
            Item Item1 = new Item("Yougurt", "YT2");
            Item1.AddPricePerPack(4, 4.95M);
            Item1.AddPricePerPack(10, 9.95M);

            Item Item2 = new Item("Sliced Ham", "SH3");
            Item2.AddPricePerPack(3, 2.99M);
            Item2.AddPricePerPack(5, 4.49M);

            Item Item3 = new Item("Toilet Rolls", "TR");
            Item3.AddPricePerPack(3, 2.95M);
            Item3.AddPricePerPack(5, 4.45M);

            Item Item4 = new Item("Beer", "BR6");
            Item4.AddPricePerPack(6, 12.99M);
            Item4.AddPricePerPack(12, 24.00M);
            Item4.AddPricePerPack(24, 46.00M);

            Item Item5 = new Item("Paper Towels", "PT3");
            Item5.AddPricePerPack(3, 2.99M);
            Item5.AddPricePerPack(6, 5.00M);
            
            Item3.AddPricePerPack(5, 7.99M);
            Item3.AddPricePerPack(10, 9.00M);

            //Creating instance of a warehouse stock List to add items to it
            StockList warehouse1 = new StockList("Warehouse 1");
            warehouse1.AddItemToStockList(Item1);
            warehouse1.AddItemToStockList(Item2);
            warehouse1.AddItemToStockList(Item3);
            warehouse1.AddItemToStockList(Item4);
            warehouse1.AddItemToStockList(Item5);
            
            //Printing warehouse 1 stock file
            Console.WriteLine("\nWarehouse 1 stocks {0}", warehouse1.PrintStockFile());

            //Creating instance of a purchase order with order id 0001 specifying which warehouse it will be ordered from
            Order order1 = new Order("0001", warehouse1);
            
            //------------INPUT------------------
            order1.AddItemToOrder(15, "YT2");
            order1.AddItemToOrder(33, "SH3");
            order1.AddItemToOrder(40, "TR");
            order1.AddItemToOrder(4, "YT2");
            order1.AddItemToOrder(113, "BR6");

            Console.WriteLine("\n--INPUT--");
            Console.WriteLine("{0}",order1.PrintOrderSummary());

            //-------------OUTPUT------------------
            Console.WriteLine("--OUTPUT--");
            foreach (OrderLineItem o in order1.Items)
            {
                Console.WriteLine(o.CalcTotalPrice()); 
            }           
        }

        //Price per pack class for Item's list
        class PricePerPack
        {
            private int _Pack;
            private decimal _Price;

            public PricePerPack(int pack, decimal price)
            {
                _Pack = pack;
                _Price = price;
            }

            public int Pack
            {
                get { return _Pack; }
                set { _Pack = value; }
            }

            public decimal Price
            {
                get { return _Price; }
                set { _Price = value; }
            }   
        }

        //Order line items for Order list
        class OrderLineItem
        {
            private Item _Item;
            private int _Quantity;

            public OrderLineItem(Item item, int quantity)
            {
                _Item = item;
                _Quantity = quantity;
            }

            public Item Item
            {
                get { return _Item; }
            }

            public int Quantity
            {
                get { return _Quantity; }
            }

            public void AddQty(int num)
            {
                _Quantity += num;
            }

            //Get total price base on total quantity
            public string CalcTotalPrice()
            {
                string output = "";
                string breakDown = "";
                string grandTotalQuote = "";
                int Counter = 0;
                int qty = _Quantity;
                decimal grandTotal = 0.00M;
                decimal totalPrice = 0.00M;

                //Get all price per pack of the item
                //Sort price per pack in descending order
                List<PricePerPack> PL = _Item.PricePerPacks.OrderByDescending(x => x.Pack).ToList();

                //Calculating grand total and price breakdown of each item
                foreach (PricePerPack p in PL)
                {
                    totalPrice = 0;
                    if(qty >= p.Pack)
                    {
                        while (qty >= p.Pack)
                        {

                            qty -= p.Pack;
                            totalPrice += p.Price;
                            Counter += 1;
                        }
                    }
                    
                    grandTotal += totalPrice;
                    if(Counter != 0)
                    {
                        breakDown += string.Format("  {1} X {0} @ ${2:C}\n", p.Pack, Counter, p.Price.ToString());
                    }
                    Counter = 0;
                }
                grandTotalQuote += string.Format("{0} {1} {2:C}\n", _Quantity, _Item.ItemNumber, grandTotal);
                output = grandTotalQuote + breakDown;
                return output;
            }           
        }

        //Item
        class Item
        {
            //Private Item members
            private string _ItemDescription;
            private string _ItemNumber;
            private List<PricePerPack> _PricePerPacks = new List<PricePerPack>();

            //Constructor
            public Item(string itemDescription, string itemNumber)
            {
                _ItemDescription = itemDescription;
                _ItemNumber = itemNumber;
            }

            //accessors and modifiers
            public string ItemDescription
            {
                get { return _ItemDescription; }
            }

            public string ItemNumber
            {
                get { return _ItemNumber; }
            }

            //Price Perpack accessor
            public ReadOnlyCollection<PricePerPack> PricePerPacks
            {
                get { return _PricePerPacks.AsReadOnly(); }
            }

            //add key value pair of price per number of pack to list of price per pack
            public void AddPricePerPack(int packs, decimal price)
            {
                bool PricePerPackExist = false;
                //_PricePerPack.Add(new KeyValuePair<int, decimal>(packs, price));
                foreach(PricePerPack p in _PricePerPacks)
                {
                    if(p.Pack == packs)
                    {
                        PricePerPackExist = true;
                    }
                }

                //Price per pack don't exist in the item yet
                if(PricePerPackExist == false)
                {
                    _PricePerPacks.Add(new PricePerPack(packs, price));
                    //Ensure that the price per pack list is in order
                    List<PricePerPack> PL = _PricePerPacks.OrderBy(x => x.Pack).ToList();
                    _PricePerPacks = PL;
                }
                else
                {
                    foreach(PricePerPack p in _PricePerPacks)
                    {
                        if(packs == p.Pack)
                        {
                            p.Price = price;
                        }
                    }
                }
            }

            //Output prices per packs of the item
            public string DisplayPricePerPack()
            {
                string output = "";
                foreach (PricePerPack p in _PricePerPacks)
                {
                    output += String.Format("{0} @ {1:C}, ", p.Pack, p.Price);
                }
                return output;
            }
        }

        class Order
        {
            //Private order members
            private string _OrderNumber;
            private string _WarehouseFrom;
            private List<Item> _warehouseStock = new List<Item>();
            private List<OrderLineItem> _Items = new List<OrderLineItem>();

            //Order Constructor
            public Order(string orderNumber, StockList warehouse)
            {
                _OrderNumber = orderNumber;
                _WarehouseFrom = warehouse.Warehouse;
                _warehouseStock = warehouse.GetList();
            }

            //Accessor and Modifiers
            public string OrderNumber
            {
                get { return _OrderNumber; }
            }
            
            public string WarehouseToOrderFrom
            {
                get { return _WarehouseFrom; }
            }

            //Price Perpack accessor
            public ReadOnlyCollection<OrderLineItem> Items
            {
                get { return _Items.AsReadOnly(); }
            }

            //output Items in warehouse
            public string ShowWarehouse()
            {
                string output = "";
                foreach(Item i in _warehouseStock)
                {
                    output += string.Format("\n--{0} {1} {2}", i.ItemDescription, i.ItemNumber, i.DisplayPricePerPack() );
                }

                return output;
            }

            //Add an item to order and quantity of the item
            public void AddItemToOrder(int qty, string ItemNumber)
            {
                Item itemToAdd = null;
                bool itemInWarehouse = false;
                bool itemAlreadyInOrder = false;
                //Check the current stock file first to ensure that the warehouse it is getting ordered from have the item on stock
                foreach(Item i in _warehouseStock)
                {
                    //if item being add exist in the warehouse
                    if (ItemNumber == i.ItemNumber)
                    {                       
                        itemToAdd = i;
                        itemInWarehouse = true;
                    }
                }

                //If items is in warehouse stock and qty being ordered is divisible by pre-pack
                if (itemInWarehouse == true )
                {
                    foreach(OrderLineItem k in _Items)
                    {
                        if(ItemNumber == k.Item.ItemNumber)
                        {
                            itemAlreadyInOrder = true;
                        }
                    }

                    //Check if item is already in the order list
                    if (itemAlreadyInOrder == true)
                    {
                        //add qty to existing order
                        foreach(OrderLineItem o in _Items)
                        {
                            if(ItemNumber == o.Item.ItemNumber)
                            {
                                o.AddQty(qty);
                            }
                        }
                    }
                    else
                    {
                        //add item or orderLineTime
                        _Items.Add(new OrderLineItem(itemToAdd, qty));
                    }
                }

                //Item in not in warehouse
                else
                {
                    Console.WriteLine("Item {0} not in stock", ItemNumber);
                }
            }

            //Output Summary of order
            public string PrintOrderSummary()
            {
                string output = "";

                //Concatenate each line item on the order to a string ouput.
                foreach(OrderLineItem o in _Items)
                {
                    output += string.Format("{0} {1}\n", o.Quantity, o.Item.ItemNumber);
                }
                return output;
            }       
        }

        //Stock List Class
        class StockList
        {
            //Private members
            private string _Warehouse;
            List<Item> _Items = new List<Item>();

            //Constructor
            public StockList(string warehouse)
            {
                _Warehouse = warehouse;
            }

            //Accessor and Modifier
            public string Warehouse
            {
                get { return _Warehouse; }
            }

            public List<Item> GetList()
            {
                return _Items;
            }

            //Add Item to list
            public void AddItemToStockList(Item item)
            {
                bool itemInStockList = false;
                
                //Check if Item being added is already in the stocklist
                foreach(Item i in _Items)
                {
                    //If Item is already in stock list, set flag to true
                    if(i.ItemNumber == item.ItemNumber)
                    {
                        itemInStockList = true;
                    }
                }

                //Item is on stock list flag to true
                if(itemInStockList == true)
                {
                    Console.WriteLine("Item {0} already in stock list", item.ItemNumber);
                }
                else
                {
                    _Items.Add(item);
                }               
            }

            //Print stock file
            public string PrintStockFile()
            {
                string output = "\n Description      |Item Number |     Price    ";
                foreach(Item i in _Items)
                {
                    output += string.Format("\n  {0, -15} | {1,-10} | {2, -40}", i.ItemDescription, i.ItemNumber, i.DisplayPricePerPack());
                }
                return output;
            }
        }
    }
}
