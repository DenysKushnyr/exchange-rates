import {create} from 'zustand';
import {persist, createJSONStorage} from 'zustand/middleware';

export const useSubsStore = create(persist((set) => ({
        currencies: [],
        subscribe: (currencyName) => set(state => ({currencies: [...state.currencies, currencyName]})),
        unsubscribe: (currencyName) => set(state =>
            ({currencies: state.currencies.filter(cName => cName !== currencyName)})),
        clear: () => set({currencies: []}),
        addRange: (subs) => set(state => ({currencies: [...state.currencies, ...subs]})),
    }),
    {
        name: 'subs',
        storage: createJSONStorage(() => localStorage)
    }));