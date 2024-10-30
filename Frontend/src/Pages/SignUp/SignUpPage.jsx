/* eslint-disable react/prop-types */
import {useEffect, useState} from 'react';
import {Link, useNavigate} from 'react-router-dom';
import {toast} from 'react-hot-toast';
import {useCookies} from "react-cookie";

export const SignupPage = ({setUserLoginCookies}) => {
    const [newUserData, setNewUserData] = useState({});
    const [submittable, setSubmittable] = useState(false);
    const navigate = useNavigate();
    const showSuccessToast = (data) => toast.success(data.message);
    const showErrorToast = (data) => toast.error(data.message);
    const [cookies] = useCookies(['user'])

    useEffect(() => {
        if (cookies.user) {
            navigate('/profile');
        }
    }, [cookies])

    useEffect(() => {
        if (
            newUserData.Name &&
            newUserData.userName &&
            newUserData.email &&
            newUserData.password &&
            newUserData.doB
        ) {
            setSubmittable(true);
        }
    }, [newUserData]);

    const handleSignupRequest = async () => {
        const response = await fetch('/api/Users/signup', {
            method: 'POST',
            headers: {
                'Content-type': 'application/json',
            },
            body: JSON.stringify({
                ...newUserData,
                registrationTime: new Date(Date.now()).toISOString()
            }),
        });
        return await response.json();

    };

    const handleSignup = (event) => {
        event.preventDefault();
        if (
            newUserData.password.match(/([a-z?'!0-9])/gi).join('') ===
            newUserData.password
        ) {
            handleSignupRequest(event).then((data) => {
                if (data.message) {
                    showErrorToast(data.message);
                } else {
                    showSuccessToast("Successfully created account!")
                    setUserLoginCookies(data.sessionToken);
                    navigate('/profile');
                }
            })
        } else {
            showErrorToast({
                message: 'Your credentials are invalid',
            });
        }

    }


    return (
        <div className='relative flex justify-center top-48'>
            <div className='relative  p-4 bg-white border border-gray-200 rounded-lg shadow sm:p-6 md:p-8'>
                <form className='w-80 max-w-screen-lg sm:w-96 m-auto'>
                    <h5 className='text-xl font-medium text-gray-900'>Registration</h5>
                    <br/>
                    <div className='flex flex-col gap-6'>
                        <label className='block text-sm font-medium text-gray-900  -mb-4'>
                            Your Name:
                        </label>
                        <input
                            onChange={(event) => {
                                setNewUserData({...newUserData, Name: event.target.value});
                            }}
                            type='text'
                            required
                            placeholder='Your name'
                            className='bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-600 dark:border-gray-500 dark:placeholder-gray-400 dark:'
                        />
                        <label className='block text-sm font-medium text-gray-900  -mb-4'>
                            Your Username:
                        </label>
                        <input
                            onChange={(event) => {
                                setNewUserData({...newUserData, userName: event.target.value});
                            }}
                            type='text'
                            required
                            placeholder='username'
                            className='bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-600 dark:border-gray-500 dark:placeholder-gray-400 dark:'
                        />
                        <label className='block  text-sm font-medium text-gray-900  -mb-4'>
                            Your Email:
                        </label>
                        <input
                            placeholder='name@mail.com'
                            className='bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-600 dark:border-gray-500 dark:placeholder-gray-400 dark:'
                            onChange={(event) => {
                                setNewUserData({...newUserData, email: event.target.value});
                            }}
                            type='email'
                            required
                        />
                        <label className='block text-sm font-medium text-gray-900  -mb-4'>
                            Password:
                        </label>
                        <input
                            type='password'
                            placeholder='********'
                            className='bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-600 dark:border-gray-500 dark:placeholder-gray-400 dark:'
                            onChange={(event) => {
                                setNewUserData({
                                    ...newUserData,
                                    password: event.target.value,
                                });
                            }}
                            required
                        />
                        <label className='block text-sm font-medium text-gray-900  -mb-4'>
                            Date of Birth:
                        </label>
                        <input
                            onChange={(event) => {
                                setNewUserData({...newUserData, doB: event.target.value});
                            }}
                            type='datetime'
                            required
                            placeholder='yyyy-mm-dd'
                            className='bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-600 dark:border-gray-500 dark:placeholder-gray-400 dark:'
                        />
                    </div>
                    <button
                        onClick={handleSignup}
                        className='w-full mt-6 bg-blue-500 hover:bg-blue-600 text-white focus:ring-4 focus:outline-none focus:ring-blue-300 font-medium rounded-lg text-sm px-5 py-2.5 text-center cursor-pointer'
                        disabled={!submittable}
                    >
                        Sign up
                    </button>
                </form>
                <div className='mt-2'>
          <span className='text-center text-slate-500 font-normal'>
            {' '}
              Already have an account?{' '}
          </span>
                    <Link to='/login' className='font-medium text-blue-700'>
                        Login!
                    </Link>
                </div>
            </div>
        </div>
    );
};