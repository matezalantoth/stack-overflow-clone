import {StrictMode, useEffect} from 'react'
import './style.css'
import {BrowserRouter, Routes, Route} from "react-router-dom"
import {CookiesProvider, useCookies} from 'react-cookie';
import {SignupPage} from "./Pages/SingUp/SignUpPage.jsx";
import ProfilePage from "./Pages/User/ProfilePage.jsx";


import WelcomePage from './Pages/Welcome/WelcomePage'
import Navbar from './components/navbar/Navbar'
import {Toaster} from "react-hot-toast";
import {LoginPage} from "./Pages/LogIn/LogIn.jsx";

export default function App() 
{
    const[cookies, setCookies] = useCookies(['user'])
    
    
    function setUserLoginCookies(user) {
        setCookies('user', user, {path: '/'})
    }
    return (
        <StrictMode>
            <CookiesProvider>
                <BrowserRouter>
                    <Navbar cookies={cookies}/>
                    <Routes>
                        <Route path="/" element={<WelcomePage cookies={cookies}/>} />
                        <Route path="/signup" element={<SignupPage cookies={cookies} setUserLoginCookies={setUserLoginCookies}/>}/>
                        <Route path="/login" element={<LoginPage cookies={cookies} setUserLoginCookies={setUserLoginCookies}/>}/>
                        <Route path="/profile" element={<ProfilePage cookies={cookies} setUserLoginCookies={setUserLoginCookies}/>}/>
                    </Routes>
                    <Toaster/>

                </BrowserRouter>
            </CookiesProvider>
        </StrictMode>
    )
}




