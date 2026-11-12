// Change "INR" here to switch the whole app's currency display.
const CURRENCY = "INR";

const moneyFormatter = new Intl.NumberFormat("en-IN", {
  style: "currency",
  currency: CURRENCY,
  maximumFractionDigits: 0,
});

export const formatMoney = (value: number | null | undefined): string =>
  value === null || value === undefined ? "—" : moneyFormatter.format(value);

export const formatPercent = (value: number | null | undefined): string =>
  value === null || value === undefined ? "—" : `${value}%`;