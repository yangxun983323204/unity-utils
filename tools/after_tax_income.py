import sys
import tkinter as tk

class TaxLevel:
    def __init__(self, min:int, max:int, rate:float, fastNum:int) -> None:
        self.min = min
        self.max = max
        self.rate = rate
        self.fastNum = fastNum

TaxTable=[
    TaxLevel(-sys.maxsize, 0, 0, 0),
    TaxLevel(0, 36000, 0.03, 0),
    TaxLevel(36000, 144000, 0.1, 2520),
    TaxLevel(144000, 300000, 0.2, 16920),
    TaxLevel(300000, 420000, 0.25, 31920),
    TaxLevel(420000, 660000, 0.3, 52920),
    TaxLevel(660000, 960000, 0.35, 85920),
    TaxLevel(960000, sys.maxsize, 0.45, 181920),
]

def FindTaxLevel(table:list[TaxLevel], ratal:int)->TaxLevel:
    for lv in table:
        if ratal > lv.min and ratal <= lv.max:
            return lv
    
    raise LookupError(f"找不到纳税额->{ratal}<-对应的税率区间")

def CalcTax(table:list[TaxLevel], income:int, deduct:int)->int:
    ratal = income - deduct
    taxLv = FindTaxLevel(table, ratal)
    tax = ratal * taxLv.rate - taxLv.fastNum
    return tax

class FundItem:
    def __init__(self, companyRate:float=0, companyAdd:int=0, selfRate:float=0, selfAdd:int=0) -> None:
        self.companyRate = companyRate
        self.companyAdd = companyAdd
        self.selfRate = selfRate
        self.selfAdd = selfAdd

    def GetCompanySum(self, companyBase:int)->float:
        return self.companyRate * companyBase + self.companyAdd
    
    def GetSelfSum(self, selfBase:int)->float:
        return self.selfRate * selfBase + self.selfAdd

class Fund51:
    def __init__(self, base5:int=0, base1:int=0, old:FundItem=None, medical:FundItem=None, job:FundItem=None, hurt:FundItem=None, birth:FundItem=None, house:FundItem=None) -> None:
        self.base5 = base5
        self.base1 = base1
        self.old = old or FundItem()
        self.medical = medical or FundItem()
        self.job = job or FundItem()
        self.hurt = hurt or FundItem()
        self.birth = birth or FundItem()
        self.house = house or FundItem()
    
    def GetCompanySum(self)->float:
        return self.old.GetCompanySum(self.base5) + self.medical.GetCompanySum(self.base5) + self.job.GetCompanySum(self.base5) + self.hurt.GetCompanySum(self.base5) + self.birth.GetCompanySum(self.base5) + self.house.GetCompanySum(self.base1)

    def GetSelfSum(self)->float:
        return self.old.GetSelfSum(self.base5) + self.medical.GetSelfSum(self.base5) + self.job.GetSelfSum(self.base5) + self.hurt.GetSelfSum(self.base5) + self.birth.GetSelfSum(self.base5) + self.house.GetSelfSum(self.base1)

class TaxDeduct:
    def __init__(self, children:int, edu:int, illness:int, house:int, rent:int, old:int, baby:int) -> None:
        self.children = children
        self.edu = edu
        self.illness = illness
        self.house = house
        self.rent = rent
        self.old = old
        self.baby = baby
    
    def GetSum(self)->int:
        return self.children + self.edu + self.illness + self.house + self.rent + self.old + self.baby

class Income:
    def __init__(self, monthIncome:int, monthCount:int, extraAdd:int, monthThreshold:int, deduct:TaxDeduct, fund:Fund51) -> None:
        self.monthIncome = monthIncome
        self.monthCount = monthCount
        self.extraAdd = extraAdd
        self.monthThreshold = monthThreshold
        self.deduct = deduct
        self.fund = fund
    
    def Calc(self)->None:
        calcMonthCount = min(self.monthCount, 12)
        self.BeforeTaxIncome = self.monthIncome * self.monthCount + self.extraAdd
        self.CompanyFundSum = self.fund.GetCompanySum() * calcMonthCount
        self.FundSum = self.fund.GetSelfSum() * calcMonthCount
        self.ThresholdSum = self.monthThreshold * calcMonthCount
        self.DeductSum = self.deduct.GetSum() * calcMonthCount

        self.TaxDeduct = self.ThresholdSum + self.DeductSum + self.FundSum
        self.Tax = CalcTax(TaxTable, self.BeforeTaxIncome, self.TaxDeduct)
        self.AfterTaxIncome = self.BeforeTaxIncome - self.Tax
        self.AfterTaxFundIncome = self.AfterTaxIncome - self.FundSum

    def Print(self)->None:
        print(f"月薪：{self.monthIncome}，月数：{self.monthCount}，社保基数：{self.fund.base5:.2f}，公积金基数：{self.fund.base1:.2f}， 年度总和：税前：{self.BeforeTaxIncome}，五险一金个人：{self.FundSum:.2f}，五险一金公司：{self.CompanyFundSum:.2f}，免征税：{self.ThresholdSum}，专项扣除：{self.DeductSum}，个税：{self.Tax:.2f}，税后：{self.AfterTaxIncome:.2f}，税后不含五险一金：{self.AfterTaxFundIncome:.2f}")

class DataHelper:
    def CreateDefaultFund51()->Fund51:
        old = FundItem(companyRate=0.2, companyAdd=0, selfRate=0.08, selfAdd=0)
        medical = FundItem(companyRate=0.1, companyAdd=0, selfRate=0.02, selfAdd=3)
        job = FundItem(companyRate=0.01, companyAdd=0, selfRate=0.002, selfAdd=0)
        hurt = FundItem(companyRate=0.02, companyAdd=0, selfRate=0, selfAdd=0)
        birth = FundItem(companyRate=0.008, companyAdd=0, selfRate=0, selfAdd=0)
        house = FundItem(companyRate=0.12, companyAdd=0, selfRate=0.12, selfAdd=0)

        inst = Fund51(old=old, medical=medical, job=job, hurt=hurt, birth=birth, house=house)
        return inst

# test
len = len(sys.argv)
if len<4:
    print("输入月薪、月数、地区年平均收入")
else:
    monthIncome = int(sys.argv[1])
    monthCount = int(sys.argv[2])

    general = int(sys.argv[3])
    base5 = max(general*0.6/12, monthIncome)
    base5 = min(base5, general*3/12)
    base1 = monthIncome

    fund51 = DataHelper.CreateDefaultFund51()
    fund51.base1 = base1
    fund51.base5 = base5
    testIncome = Income(monthIncome,monthCount,0,5000,TaxDeduct(0,0,0,0,0,0,0),fund51)
    testIncome.Calc()
    testIncome.Print()
