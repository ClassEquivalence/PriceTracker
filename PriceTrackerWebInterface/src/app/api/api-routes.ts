

const hostAddress = 'https://localhost:7170';

export const ApiRoutes = {
  productByCitilinkCode: (code: string) => `${hostAddress}/api/Merch/citilink/${code}`,
  productById: (id: number) => `${hostAddress}/api/Merch/${id}`,
};
