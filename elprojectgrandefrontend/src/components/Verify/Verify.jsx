import {useState} from "react";
import {useCookies} from "react-cookie";
import {useNavigate} from "react-router-dom";

export default function Verify({setDetails, setVerified}) {
    const [userDetails, setUserDetails] = useState(null);
    const [cookies] = useCookies(['user'])
    const navigate = useNavigate()
    const handleVerify = async (e) => {
        e.preventDefault();


        const response = await fetch('/api/Users/VerifyUser', {
            method: 'POST',
            headers: {
                'Content-type': 'application/json',
                'Authorization': "Bearer " + cookies.user
            },
            body: JSON.stringify(userDetails),
        });
        return await response.json();


    };


    return (

        <div className='relative flex justify-center top-48'>
            <div
                className='relative w-full max-w-sm p-4 bg-white border border-gray-200 rounded-lg shadow sm:p-6 md:p-8 text-black'>
                <form className='space-y-6' action='#'>
                    <div>
                        <label className='block mb-2 text-sm font-medium text-gray-900 '>
                            Your password
                        </label>
                        <input
                            onChange={(event) => {
                                setUserDetails(event.target.value);
                            }}
                            type='password'
                            placeholder='••••••••'
                            className='bg-gray-50 border border-gray-300 text-white text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-600 dark:border-gray-500 dark:placeholder-gray-400 '
                            required
                        />
                    </div>
                    <button
                        onClick={async (event) => {
                            const data = await handleVerify(event)
                            console.log(data)

                            if (!data.verified) {

                                navigate('/profile')
                                return

                            }
                            setVerified(() => true)
                            setDetails({email: data.email, password: null})
                        }}
                        type='submit'
                        className='w-full text-white bg-blue-500 hover:bg-blue-600 focus:ring-4 focus:outline-nonefont-medium rounded-lg text-sm px-5 py-2.5 text-center'
                    >
                        Login to your account
                    </button>
                </form>
            </div>
        </div>


    )
}