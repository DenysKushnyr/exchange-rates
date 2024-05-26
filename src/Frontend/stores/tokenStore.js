import {create} from 'zustand';
import {persist, createJSONStorage} from 'zustand/middleware';

export const useAuthStore = create(persist((set) => ({
        token: null,
        email: null,
        isAuthenticated: false,
        login: (token, email) => set({token, email, isAuthenticated: true}),
        logout: () => set({token: null, email: null, isAuthenticated: false}),
    }),
    {
        name: 'token',
        storage: createJSONStorage(() => localStorage)
    }));