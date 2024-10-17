import {StrictMode, useState} from 'react'
import './style.css'
import {BrowserRouter, Route, Routes} from "react-router-dom"
import {CookiesProvider, useCookies} from 'react-cookie';
import {SignupPage} from "./Pages/SignUp/SignUpPage.jsx";
import ProfilePage from "./Pages/User/ProfilePage.jsx";


import WelcomePage from './Pages/WelcomePage/WelcomePage'
import Navbar from './components/navbar/Navbar'
import {Toaster} from "react-hot-toast";
import {LoginPage} from "./Pages/LogIn/LogIn.jsx";

import QuestionPage from "./Pages/SingleQuestion/QuestionPage.jsx";

import AskQuestion from "./Pages/AskQuestion/AskQuestion.jsx";
import PublicUser from "./Pages/User/PublicUser.jsx";
import AdminPage from "./Pages/Admin/AdminPage.jsx";
import TagsPage from "./Pages/TagsPage/TagsPage.jsx";


export default function App() {
    const [cookies, setCookies] = useCookies(['user'])
    const [searchQuestion, setsearchQuestion] = useState([])
    const [normalQuestion, setNormalQuestion] = useState([])
    const [tags, setTags] = useState([])

    function setUserLoginCookies(user) {
        setCookies('user', user, {path: '/'})
    }

    return (

        <StrictMode>
            <CookiesProvider>
                <BrowserRouter>
                    <Navbar setUserLoginCookies={setUserLoginCookies} setsearchQuestion={setsearchQuestion}
                            normalQuestion={normalQuestion}/>
                    <Routes>
                        <Route path="/"
                               element={<WelcomePage searchQuestion={searchQuestion} normalQuestion={normalQuestion}
                                                     setNormalQuestion={setNormalQuestion}
                                                     setsearchQuestion={setsearchQuestion}
                                                     setUserLoginCookies={setUserLoginCookies}/>}/>
                        <Route path="/signup"
                               element={<SignupPage setUserLoginCookies={setUserLoginCookies}/>}/>
                        <Route path="/login"
                               element={<LoginPage setUserLoginCookies={setUserLoginCookies}/>}/>
                        <Route path="/askquestion" element={<AskQuestion setUserLoginCookies={setUserLoginCookies}/>}/>
                        <Route path="/profile"
                               element={<ProfilePage setUserLoginCookies={setUserLoginCookies}/>}/>
                        <Route path='question/:questionId'
                               element={<QuestionPage setUserLoginCookies={setUserLoginCookies}/>}/>
                        <Route path='/user/:userName'
                               element={<PublicUser setUserLoginCookies={setUserLoginCookies}/>}/>
                        <Route path='/admin' element={<AdminPage setUserLoginCookies={setUserLoginCookies}/>}/>
                        <Route path='/tags' element={<TagsPage tags={tags} setTags={setTags}
                                                               setUserLoginCookies={setUserLoginCookies}/>}/>
                    </Routes>
                    <Toaster/>

                </BrowserRouter>
            </CookiesProvider>
        </StrictMode>
    )
}




