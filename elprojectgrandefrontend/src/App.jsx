import {StrictMode, useState} from 'react'
import './style.css'
import {BrowserRouter, Route, Routes} from "react-router-dom"
import {CookiesProvider, useCookies} from 'react-cookie';
import {SignupPage} from "./Pages/SignUp/SignUpPage.jsx";
import ProfilePage from "./Pages/User/ProfilePage.jsx";


import WelcomePage from './Pages/Welcome/WelcomePage'
import Navbar from './components/navbar/Navbar'
import {Toaster} from "react-hot-toast";
import {LoginPage} from "./Pages/LogIn/LogIn.jsx";

import QuestionPage from "./Pages/SingleQuestion/QuestionPage.jsx";

import AskQuestion from "./Pages/AskQuestion/AskQuestion.jsx";
import PublicUser from "./Pages/User/PublicUser.jsx";
import AdminPage from "./Pages/Admin/AdminPage.jsx";
import TagsPage from "./Pages/TagsPage/TagsPage.jsx";
import TagPage from "./Pages/TagsPage/TagPage.jsx";


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
                                                     setsearchQuestion={setsearchQuestion}/>}/>
                        <Route path="/signup"
                               element={<SignupPage cookies={cookies} setUserLoginCookies={setUserLoginCookies}/>}/>
                        <Route path="/login"
                               element={<LoginPage cookies={cookies} setUserLoginCookies={setUserLoginCookies}/>}/>
                        <Route path="/askquestion" element={<AskQuestion cookies={cookies} tags={tags}/>}/>
                        <Route path="/profile"
                               element={<ProfilePage cookies={cookies} setUserLoginCookies={setUserLoginCookies}/>}/>
                        <Route path='question/:questionId' element={<QuestionPage cookies={cookies}/>}/>
                        <Route path='/user/:userName' element={<PublicUser/>}/>
                        <Route path='/admin' element={<AdminPage cookies={cookies}/>}/>
                        <Route path='/tags' element={<TagsPage cookies={cookies} tags={tags} setTags={setTags}/>}/>
                        <Route path='tag/:tagId' element={<TagPage cookies={cookies}/>}/>
                    </Routes>
                    <Toaster/>

                </BrowserRouter>
            </CookiesProvider>
        </StrictMode>
    )
}




