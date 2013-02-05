using System;
using System.Linq;
using DataAccess.Company;
using DataAccess.Company.Requests;
using Domain.Company;
using Domain.Switching;
using Glue;
using Glue.Converters;

namespace DataAccess
{
    public class MappingFactory
    {
        public Mapping<T1, T2> GetMapping<T1, T2>()
        {
            // We are essentially going to use a dynamic programming trick here.
            object retMapping = null;
            TypeSwitch.Do(typeof(T1), typeof(T2),
                TypeSwitch.Case<DbDate, DateTime>(() => retMapping = DbDateToDateTime),

                /* Most mappings should be fully bidirectional, so you'll have to set up two cases to return the right one regardless of order.
                 * TypeSwitch could be modified to do this automatically, but I haven't gotten around to that yet.  Also requires some minor changes
                 * to the interface and base adapter, but nothing too serious.  For now, though, this does a good job of showing that yes, these things
                 * are bidirectional.*/
                TypeSwitch.Case<DbAddress, DomainAddress>(() => retMapping = DbAddressToDomainAddress),
                TypeSwitch.Case<DomainAddress, DbAddress>(() => retMapping = DbAddressToDomainAddress),

                TypeSwitch.Case<PaymentInfo, DomainPaymentInfo>(() => retMapping = PaymentInfoToDomainPaymentInfo),
                TypeSwitch.Case<DomainPaymentInfo, PaymentInfo>(() => retMapping = PaymentInfoToDomainPaymentInfo),

                TypeSwitch.Case<AddPaymentRequest, NewPayment>(() => retMapping = AddPaymentRequestToNewPayment),
                TypeSwitch.Case<NewPayment, AddPaymentRequest>(() => retMapping = AddPaymentRequestToNewPayment));

            if (retMapping == null)
            {
                // This is a last-ditch, but also an acceptable solution for simple mappings like Customer <-> DomainCustomer.
                var defaultMapping = new Mapping<T1, T2>();

                /* Does what it says on the tin.  If two objects have the same names it will auto relate them.  Glue can also do some
                 * basic flattening, from string to int or whatever, but generally it is best to have it be strictly equal for clarity's sake.
                 */
                defaultMapping.AutoRelateEqualNames();


                retMapping = defaultMapping;
            }
            
            /* Retmapping retains its identity even though it is cast as object.  Since, heuristically, we know what T1 and T2 are, this mapping is 
             * essentially the same as going int v = (int)1.  Point is, it works great!
             */
            return (Mapping<T1, T2>) retMapping;


            /* Note: This sort of embedded delegate stuff is perfectly acceptable in TypeSwitch, but it's ugly as sin and hard to follow.
                 * Right now the ugly is moved to TypeSwitch by using a pretty gross DualCaseInfo class.  It's way better than the alternative though.
                 * TypeSwitch.Case<DbDate>(() =>
                    TypeSwitch.Do(
                        typeof(T2),
                        TypeSwitch.Case<DateTime>(() => retMapping = DbDateToDateTime)));*/
        }

        private Mapping<DbDate, DateTime> _dbDateToDateTime;
        public Mapping<DbDate, DateTime> DbDateToDateTime
        {
            get 
            {
                if (_dbDateToDateTime == null)
                {
                    /* Sometimes you have a struct like DateTime can't be written to after creation, so we must use a creator.
                     * These tell Glue how to create an object, given an instance of another object.
                     * Glue hates creating objects during normal mappings - the framework has a philosophical stand
                     * that object creation should be done by factories, not mappers.  Creators are
                     * therefore essential if you need object creation (which is how a lot of people use 
                     * mapping frameworks like this one).  These creators can be used during Mapping.Map().
                     */
                    _dbDateToDateTime = new Mapping<DbDate, DateTime>(
                        dateTime => new DbDate {Date = dateTime.ToShortDateString()},
                        bldrDate => DateTime.Parse(bldrDate.Date));
                }
                return _dbDateToDateTime;

                /* This entire thing can be shortened to a one liner as follows, but I leave it longhand for instructional purposes:
                 * return _dbDateToDateTime ?? (_dbDateToDateTime = new Mapping<DbDate, DateTime>(
                                dateTime => new DbDate {Date = dateTime.ToShortDateString()},
                                bldrDate => DateTime.Parse(bldrDate.Date)));
                 */
            }
        }

        private Mapping<DbAddress, DomainAddress> _dbAddressToDomainAddress;
        public Mapping<DbAddress, DomainAddress> DbAddressToDomainAddress 
        {
            get 
            {
                if (_dbAddressToDomainAddress == null)
                {
                    _dbAddressToDomainAddress = new Mapping<DbAddress, DomainAddress>();

                    // .Relate relations are bidirectional.  The accessed resources must be both READABLE and WRITEABLE.
                    _dbAddressToDomainAddress.Relate(dbAddress => dbAddress.Address1, address => address.Address1);
                    _dbAddressToDomainAddress.Relate(dbAddress => dbAddress.Address2, address => address.Address2);
                    _dbAddressToDomainAddress.Relate(dbAddress => dbAddress.City, address => address.City);
                    _dbAddressToDomainAddress.Relate(dbAddress => dbAddress.State, address => address.State);

                    /* .RelateTowardRight or .RelateTowardLeft are unidirectional.  The FROM resource must be READABLE.
                     * The TO resource must be WRITABLE.  This means that it's entirely possible for the FROM resource to be
                     * a function, like so:
                     */
                    _dbAddressToDomainAddress.RelateTowardsRight(dbAddress => dbAddress.GetFullZip(), address => address.Zip);
                    _dbAddressToDomainAddress.RelateTowardsLeft(dbAddress => dbAddress.Zip5, address => address.Get5Zip());
                    _dbAddressToDomainAddress.RelateTowardsLeft(dbAddress => dbAddress.Zip4, address => address.Get4Zip());
                }
                return _dbAddressToDomainAddress;

                // The same logic we used in the .Relate statements can go into Creators as well if you want or need to make them.
            }
        }

        /* This is an example of a fully mature mapping, with both Creator and Mapper.  This is not always required,
         * but it is never a bad idea to make the whole thing.  The worst thing that can happen is you increase the size of your
         * compiled DLL a few kilobytes by including some delegates and Relate invocations you won't really use.
         */
        private Mapping<PaymentInfo, DomainPaymentInfo> _paymentInfoToDomainPaymentInfo;
        public Mapping<PaymentInfo, DomainPaymentInfo> PaymentInfoToDomainPaymentInfo 
        { 
            get 
            {
                if (_paymentInfoToDomainPaymentInfo == null)
                {
                    _paymentInfoToDomainPaymentInfo = new Mapping<PaymentInfo, DomainPaymentInfo>(
                        domainPaymentInfo => new PaymentInfo
                            {
                                CurrencyType = _currencyToCurrencyConverter.MapTowardsLeft(domainPaymentInfo.CurrencyType),
                                NextPaymentDate = DbDateToDateTime.Map(domainPaymentInfo.NextPaymentDate),
                                SerialNumber = domainPaymentInfo.SerialNumber,
                                SourceAccount = domainPaymentInfo.SourceAccount,
                                SpecialPaymentDates = domainPaymentInfo.SpecialPaymentDates.Select(s => DbDateToDateTime.Map(s)).ToArray()
                            },
                        paymentInfo => new DomainPaymentInfo
                            {
                                CurrencyType = _currencyToCurrencyConverter.MapTowardsRight(paymentInfo.CurrencyType),
                                NextPaymentDate = DbDateToDateTime.Map(paymentInfo.NextPaymentDate),
                                SerialNumber = paymentInfo.SerialNumber,
                                SourceAccount = paymentInfo.SourceAccount,
                                SpecialPaymentDates = paymentInfo.SpecialPaymentDates.Select(s => DbDateToDateTime.Map(s))
                            }
                    );

                    // If you have two properties of unlike types, like these two unequal enums, you can pass in your own Converter to tell Glue how to do it.
                    _paymentInfoToDomainPaymentInfo.Relate(paymentInfo => paymentInfo.CurrencyType, domainPaymentInfo => domainPaymentInfo.CurrencyType, _currencyToCurrencyConverter);
                    // You can also pass in your own sub-Mapping.  Note this is DbDate and DateTime.
                    _paymentInfoToDomainPaymentInfo.Relate(paymentInfo => paymentInfo.NextPaymentDate, domainPaymentInfo => domainPaymentInfo.NextPaymentDate, _dbDateToDateTime);

                    _paymentInfoToDomainPaymentInfo.Relate(paymentInfo => paymentInfo.SerialNumber, domainPaymentInfo => domainPaymentInfo.SerialNumber);
                    _paymentInfoToDomainPaymentInfo.Relate(paymentInfo => paymentInfo.SourceAccount, domainPaymentInfo => domainPaymentInfo.SourceAccount);

                    _paymentInfoToDomainPaymentInfo.RelateTowardsLeft(paymentInfo => paymentInfo.SpecialPaymentDates.AsEnumerable(), domainPaymentInfo => domainPaymentInfo.SpecialPaymentDates, _dbDateToDateTime);
                    _paymentInfoToDomainPaymentInfo.RelateTowardsRight(paymentInfo => paymentInfo.SpecialPaymentDates, domainPaymentInfo => domainPaymentInfo.SpecialPaymentDates.ToArray(), _dbDateToDateTime);

                }
                return _paymentInfoToDomainPaymentInfo;
            }
        }


        private Mapping<AddPaymentRequest, NewPayment> _addPaymentRequestToNewPayment; 
        public Mapping<AddPaymentRequest, NewPayment> AddPaymentRequestToNewPayment 
        { 
            get 
            {
                if (_addPaymentRequestToNewPayment == null)
                {
                    /* While conveinent to do, if this becomes rampant in a large file this can easily be something that is overlooked.
                     * Keep good unit tests on each of your manually defined mappings!  That's half the reason for using Glue in the first place!
                     */
                    var customerAutoRelate = new Mapping<Customer, DomainCustomer>();
                    customerAutoRelate.AutoRelateEqualNames();

                    _addPaymentRequestToNewPayment = new Mapping<AddPaymentRequest, NewPayment>
                        (
                        newPayment => new AddPaymentRequest
                            {
                                PayingCustomer = customerAutoRelate.Map(newPayment.PayingCustomer),
                                Payment = PaymentInfoToDomainPaymentInfo.Map(newPayment.PaymentInfo)
                            },
                        addPaymentRequest => new NewPayment 
                            { 
                                PayingCustomer = customerAutoRelate.Map(addPaymentRequest.PayingCustomer), 
                                PaymentInfo = PaymentInfoToDomainPaymentInfo.Map(addPaymentRequest.Payment) 
                            }
                        );

                    _addPaymentRequestToNewPayment.Relate(addPaymentRequest => addPaymentRequest.PayingCustomer, newPayment => newPayment.PayingCustomer, customerAutoRelate);
                    _addPaymentRequestToNewPayment.Relate(addPaymentRequest => addPaymentRequest.Payment, newPayment => newPayment.PaymentInfo, PaymentInfoToDomainPaymentInfo);
                }
                return _addPaymentRequestToNewPayment;
            }
        }

        // These two enums don't nessecerally have the same order of things, so we can relate them on name instead.
        readonly QuickConverter<Company.Enums.Currency, Domain.Company.Enums.Currency> _currencyToCurrencyConverter = new QuickConverter<Company.Enums.Currency, Domain.Company.Enums.Currency>(
                   fromEnum => (Domain.Company.Enums.Currency)Enum.Parse(typeof(Domain.Company.Enums.Currency), fromEnum.ToString(), true)
                   , toEnum => (Company.Enums.Currency)Enum.Parse(typeof(Company.Enums.Currency), toEnum.ToString(), true));
    }
}
